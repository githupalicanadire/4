import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
} from "react";
import { UserManager, User } from "oidc-client-ts";
import api from "../services/api";

const config = {
  authority: "http://localhost:6007",
  client_id: "shopping-spa",
  redirect_uri: `${window.location.origin}/callback`,
  response_type: "code",
  scope: "openid profile email roles shopping_api catalog basket ordering",
  post_logout_redirect_uri: window.location.origin,
  automaticSilentRenew: true,
  silent_redirect_uri: `${window.location.origin}/silent-callback`,
};

const userManager = new UserManager(config);

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

  const login = async (username, password) => {
    try {
      setLoading(true);

      // Identity Server'dan token al (Resource Owner Password Grant)
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
        console.error("Token error:", errorData);
        throw new Error(errorData.error_description || "Giriş başarısız");
      }

      const tokenData = await tokenResponse.json();
      console.log("🔑 Token received:", {
        access_token: tokenData.access_token ? "✅" : "❌",
        id_token: tokenData.id_token ? "✅" : "❌",
        expires_in: tokenData.expires_in,
      });

      // UserInfo endpoint'inden kullanıcı bilgilerini al
      const userInfoResponse = await fetch(
        "http://localhost:6007/connect/userinfo",
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${tokenData.access_token}`,
            "Content-Type": "application/json",
          },
        },
      );

      if (!userInfoResponse.ok) {
        const errorText = await userInfoResponse.text();
        console.error("UserInfo error:", errorText);
        throw new Error("Kullanıcı bilgileri alınamadı");
      }

      const userInfo = await userInfoResponse.json();
      console.log("👤 User info received:", userInfo);

      // Kullanıcı nesnesini oluştur
      const userData = {
        // Token bilgileri
        access_token: tokenData.access_token,
        id_token: tokenData.id_token,
        token_type: tokenData.token_type || "Bearer",
        expires_at: Date.now() + tokenData.expires_in * 1000,
        refresh_token: tokenData.refresh_token,

        // Kullanıcı profil bilgileri
        sub: userInfo.sub,
        username: userInfo.preferred_username || username,
        email: userInfo.email,
        firstName: userInfo.given_name || "",
        lastName: userInfo.family_name || "",
        name: userInfo.name || userInfo.preferred_username || username,
        roles: userInfo.role
          ? Array.isArray(userInfo.role)
            ? userInfo.role
            : [userInfo.role]
          : [],

        // Raw data
        profile: userInfo,
        session_state: tokenData.session_state,
      };

      // Local storage'a kaydet
      localStorage.setItem("user", JSON.stringify(userData));
      localStorage.setItem("access_token", tokenData.access_token);

      setUser(userData);

      // API çağrıları için default header'ı ayarla
      api.defaults.headers.common["Authorization"] =
        `Bearer ${tokenData.access_token}`;

      console.log("✅ Login successful for user:", userData.username);
      return { success: true, user: userData };
    } catch (error) {
      console.error("❌ Login error:", error);
      return {
        success: false,
        message: error.message || "Giriş yapılırken hata oluştu",
      };
    } finally {
      setLoading(false);
    }
  };

  const logout = useCallback(async () => {
    try {
      console.log("🚪 Logging out user...");

      // Clear local storage
      localStorage.removeItem("user");
      localStorage.removeItem("access_token");

      // Clear API headers
      delete api.defaults.headers.common["Authorization"];

      // Clear user state
      setUser(null);

      // Optional: Call Identity Server logout endpoint
      if (user?.access_token) {
        try {
          await fetch("http://localhost:6007/connect/endsession", {
            method: "POST",
            headers: {
              Authorization: `Bearer ${user.access_token}`,
            },
          });
        } catch (error) {
          console.warn("End session failed:", error);
        }
      }

      console.log("✅ Logout successful");
    } catch (error) {
      console.error("❌ Logout error:", error);
    }
  }, [user]);

  const handleCallback = async () => {
    try {
      const user = await userManager.signinRedirectCallback();

      if (user?.access_token) {
        // Store user data
        localStorage.setItem("user", JSON.stringify(user));
        localStorage.setItem("access_token", user.access_token);

        setUser(user);
        api.defaults.headers.common["Authorization"] =
          `Bearer ${user.access_token}`;
      }

      return { success: true, user };
    } catch (error) {
      console.error("Callback error:", error);
      return {
        success: false,
        message: "Giriş işlemi tamamlanamadı",
      };
    }
  };

  const isAuthenticated = () => {
    return !!user && !!user.access_token && user.expires_at > Date.now();
  };

  const getCurrentUser = () => {
    return user?.name || user?.username || user?.profile?.name || "guest";
  };

  const getCurrentCustomerId = () => {
    return user?.sub || user?.profile?.sub || null;
  };

  const getAccessToken = () => {
    return user?.access_token || null;
  };

  const getUserRoles = () => {
    return user?.roles || [];
  };

  const hasRole = (role) => {
    return getUserRoles().includes(role);
  };

  // Check for existing session on app load
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        setLoading(true);

        // Check localStorage first
        const storedUser = localStorage.getItem("user");
        const storedToken = localStorage.getItem("access_token");

        if (storedUser && storedToken) {
          const userData = JSON.parse(storedUser);

          // Check if token is still valid
          if (userData.expires_at && userData.expires_at > Date.now()) {
            console.log("🔄 Restoring user session for:", userData.username);
            setUser(userData);
            api.defaults.headers.common["Authorization"] =
              `Bearer ${storedToken}`;
          } else {
            console.log("⏰ Stored session expired, clearing...");
            localStorage.removeItem("user");
            localStorage.removeItem("access_token");
          }
        }

        // Try OIDC user manager as fallback
        try {
          const oidcUser = await userManager.getUser();
          if (oidcUser && oidcUser.access_token && !oidcUser.expired) {
            console.log("🔄 Found valid OIDC session");
            setUser(oidcUser);
            api.defaults.headers.common["Authorization"] =
              `Bearer ${oidcUser.access_token}`;
          }
        } catch (oidcError) {
          console.log("ℹ️ No OIDC session found");
        }
      } catch (error) {
        console.error("❌ Error initializing auth:", error);
      } finally {
        setLoading(false);
      }
    };

    initializeAuth();
  }, []);

  const value = {
    user,
    loading,
    login,
    logout,
    handleCallback,
    isAuthenticated,
    getCurrentUser,
    getCurrentCustomerId,
    getAccessToken,
    getUserRoles,
    hasRole,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
