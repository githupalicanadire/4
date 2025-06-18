import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
} from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Token management
  const getStoredToken = useCallback(() => {
    return localStorage.getItem("access_token");
  }, []);

  const getStoredUser = useCallback(() => {
    const userData = localStorage.getItem("user");
    return userData ? JSON.parse(userData) : null;
  }, []);

  const clearStoredAuth = useCallback(() => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("user");
    localStorage.removeItem("refresh_token");
    delete api.defaults.headers.common["Authorization"];
  }, []);

  const decodeJWTExpiry = useCallback((token) => {
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split("")
          .map(function (c) {
            return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
          })
          .join(""),
      );
      const decoded = JSON.parse(jsonPayload);
      return decoded.exp ? decoded.exp * 1000 : null; // Convert to milliseconds
    } catch (error) {
      console.error("Error decoding JWT:", error);
      return null;
    }
  }, []);

  const storeAuthData = useCallback(
    (userData, tokenData) => {
      // Store tokens
      localStorage.setItem("access_token", tokenData.access_token);
      if (tokenData.refresh_token) {
        localStorage.setItem("refresh_token", tokenData.refresh_token);
      }

      // Get actual expiry from JWT token
      const actualExpiry = decodeJWTExpiry(tokenData.access_token);

      // Store user data
      const enrichedUser = {
        // Token info
        access_token: tokenData.access_token,
        refresh_token: tokenData.refresh_token,
        expires_at: actualExpiry || Date.now() + tokenData.expires_in * 1000, // Use JWT exp or fallback
        token_type: tokenData.token_type || "Bearer",

        // User profile from JWT or UserInfo
        sub: userData.sub,
        username: userData.preferred_username || userData.username,
        email: userData.email,
        firstName: userData.given_name,
        lastName: userData.family_name,
        name: userData.name,
        roles: Array.isArray(userData.role)
          ? userData.role
          : [userData.role].filter(Boolean),

        // Session info
        loginTime: Date.now(),
        profile: userData,
      };

      localStorage.setItem("user", JSON.stringify(enrichedUser));

      // Set API default header
      api.defaults.headers.common["Authorization"] =
        `Bearer ${tokenData.access_token}`;

      console.log("📝 Stored auth data:", {
        username: enrichedUser.username,
        expires_at: new Date(enrichedUser.expires_at).toLocaleString(),
        hasToken: !!enrichedUser.access_token,
      });

      return enrichedUser;
    },
    [decodeJWTExpiry],
  );

  const isTokenExpired = useCallback((user) => {
    if (!user?.expires_at) return true;

    const now = Date.now();
    const expiry = user.expires_at;
    const isExpired = now >= expiry;

    console.log("⏰ Token expiry check:", {
      now: new Date(now).toLocaleString(),
      expiry: new Date(expiry).toLocaleString(),
      isExpired,
      timeLeft: isExpired
        ? 0
        : Math.round((expiry - now) / 1000 / 60) + " minutes",
    });

    return isExpired;
  }, []);

  const login = async (username, password) => {
    try {
      setLoading(true);
      setError(null);

      console.log("🔐 Starting login process...");

      // Get token from Identity Server
      const tokenResponse = await fetch("http://localhost:6007/connect/token", {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: new URLSearchParams({
          grant_type: "password",
          username: username,
          password: password,
          client_id: "shopping-spa",
          scope:
            "openid profile email roles shopping_api catalog basket ordering",
        }),
      });

      if (!tokenResponse.ok) {
        const errorData = await tokenResponse.json();
        throw new Error(errorData.error_description || "Giriş başarısız");
      }

      const tokenData = await tokenResponse.json();
      console.log("✅ Token received successfully");

      // Get user info from UserInfo endpoint
      const userInfoResponse = await fetch(
        "http://localhost:6007/connect/userinfo",
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${tokenData.access_token}`,
          },
        },
      );

      if (!userInfoResponse.ok) {
        throw new Error("Kullanıcı bilgileri alınamadı");
      }

      const userData = await userInfoResponse.json();
      console.log("✅ User info received successfully");

      // Store auth data and set user
      const enrichedUser = storeAuthData(userData, tokenData);
      setUser(enrichedUser);

      console.log(`✅ Login successful for: ${enrichedUser.username}`);
      return { success: true, user: enrichedUser };
    } catch (error) {
      console.error("❌ Login failed:", error);
      setError(error.message);
      return { success: false, message: error.message };
    } finally {
      setLoading(false);
    }
  };

  const logout = useCallback(async () => {
    try {
      console.log("🚪 Logging out...");

      // Clear all auth data
      clearStoredAuth();
      setUser(null);
      setError(null);

      // Optional: Notify Identity Server (fire and forget)
      try {
        const token = getStoredToken();
        if (token) {
          fetch("http://localhost:6007/connect/endsession", {
            method: "POST",
            headers: { Authorization: `Bearer ${token}` },
          }).catch(() => {}); // Ignore errors
        }
      } catch (error) {
        // Ignore Identity Server logout errors
      }

      console.log("✅ Logout successful");
    } catch (error) {
      console.error("❌ Logout error:", error);
    }
  }, [clearStoredAuth, getStoredToken]);

  const refreshToken = useCallback(async () => {
    try {
      const refreshToken = localStorage.getItem("refresh_token");
      if (!refreshToken) {
        throw new Error("No refresh token available");
      }

      const response = await fetch("http://localhost:6007/connect/token", {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: new URLSearchParams({
          grant_type: "refresh_token",
          refresh_token: refreshToken,
          client_id: "shopping-spa",
        }),
      });

      if (!response.ok) {
        throw new Error("Token refresh failed");
      }

      const tokenData = await response.json();

      // Update stored tokens
      const currentUser = getStoredUser();
      if (currentUser) {
        const updatedUser = storeAuthData(currentUser.profile, tokenData);
        setUser(updatedUser);
        console.log("✅ Token refreshed successfully");
        return true;
      }

      return false;
    } catch (error) {
      console.error("❌ Token refresh failed:", error);
      // Force logout on refresh failure
      await logout();
      return false;
    }
  }, [getStoredUser, storeAuthData, logout]);

  // Check authentication status
  const isAuthenticated = useCallback(() => {
    const currentUser = user || getStoredUser();
    const hasUser = !!currentUser;
    const hasToken = !!currentUser?.access_token;
    const tokenExpired = currentUser ? isTokenExpired(currentUser) : true;

    console.log("🔍 Auth Check:", {
      hasUser,
      hasToken,
      tokenExpired,
      username: currentUser?.username,
      expires_at: currentUser?.expires_at,
      now: Date.now(),
    });

    const isAuth = hasUser && hasToken && !tokenExpired;
    console.log("✅ isAuthenticated result:", isAuth);

    return isAuth;
  }, [user, getStoredUser, isTokenExpired]);

  // Get current user info
  const getCurrentUser = useCallback(() => {
    const currentUser = user || getStoredUser();
    return currentUser?.name || currentUser?.username || null;
  }, [user, getStoredUser]);

  const getCurrentUserId = useCallback(() => {
    const currentUser = user || getStoredUser();
    return currentUser?.sub || null;
  }, [user, getStoredUser]);

  const getUserRoles = useCallback(() => {
    const currentUser = user || getStoredUser();
    return currentUser?.roles || [];
  }, [user, getStoredUser]);

  const hasRole = useCallback(
    (role) => {
      return getUserRoles().includes(role);
    },
    [getUserRoles],
  );

  const getAccessToken = useCallback(() => {
    const currentUser = user || getStoredUser();
    return currentUser?.access_token || null;
  }, [user, getStoredUser]);

  // Initialize auth state on app load
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        setLoading(true);

        const storedUser = getStoredUser();
        const storedToken = getStoredToken();

        if (storedUser && storedToken) {
          if (isTokenExpired(storedUser)) {
            console.log("⏰ Token expired, attempting refresh...");
            const refreshed = await refreshToken();
            if (!refreshed) {
              console.log("❌ Token refresh failed, clearing session");
              clearStoredAuth();
            }
          } else {
            console.log("🔄 Restoring valid session");
            setUser(storedUser);
            api.defaults.headers.common["Authorization"] =
              `Bearer ${storedToken}`;
          }
        } else {
          console.log("ℹ️ No stored session found");
        }
      } catch (error) {
        console.error("❌ Auth initialization error:", error);
        clearStoredAuth();
      } finally {
        setLoading(false);
      }
    };

    initializeAuth();
  }, [
    getStoredUser,
    getStoredToken,
    isTokenExpired,
    refreshToken,
    clearStoredAuth,
  ]);

  // Auto token refresh
  useEffect(() => {
    if (!user || !isAuthenticated()) return;

    const checkTokenExpiry = () => {
      if (isTokenExpired(user)) {
        console.log("⏰ Token will expire soon, refreshing...");
        refreshToken();
      }
    };

    // Check every 5 minutes
    const interval = setInterval(checkTokenExpiry, 5 * 60 * 1000);

    return () => clearInterval(interval);
  }, [user, isAuthenticated, isTokenExpired, refreshToken]);

  const value = {
    // State
    user,
    loading,
    error,

    // Actions
    login,
    logout,
    refreshToken,

    // Getters
    isAuthenticated,
    getCurrentUser,
    getCurrentUserId,
    getUserRoles,
    hasRole,
    getAccessToken,

    // Utilities
    clearError: () => setError(null),
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
