import React, { useState, useRef, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import "./Header.css";

const Header = () => {
  const { user, isAuthenticated, logout, loading, getUserRoles } = useAuth();
  const [showUserMenu, setShowUserMenu] = useState(false);
  const userMenuRef = useRef(null);
  const navigate = useNavigate();

  // Close user menu when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (userMenuRef.current && !userMenuRef.current.contains(event.target)) {
        setShowUserMenu(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const isAuth = isAuthenticated();
  const userRoles = getUserRoles();
  const isAdmin = userRoles.includes("admin");

  const handleLogout = async () => {
    try {
      await logout();
      setShowUserMenu(false);
      navigate("/", { replace: true });
    } catch (error) {
      console.error("Logout error:", error);
    }
  };

  return (
    <header className="header">
      <div className="container">
        <div className="header-content">
          <Link to="/" className="logo">
            🧸 ToyLand 🎮
          </Link>

          <nav className="nav">
            <Link to="/" className="nav-link">
              🏠 Ana Sayfa
            </Link>
            <Link to="/products" className="nav-link">
              🎁 Oyuncaklar
            </Link>

            {isAuth && (
              <>
                <Link to="/cart" className="nav-link">
                  🛒 Sepetim
                </Link>
                <Link to="/orders" className="nav-link">
                  📦 Siparişlerim
                </Link>
              </>
            )}

            {isAuth && isAdmin && (
              <Link to="/admin" className="nav-link admin-link">
                ⚙️ Yönetim
              </Link>
            )}

            {process.env.NODE_ENV === "development" && isAuth && (
              <>
                <Link to="/debug" className="nav-link debug-link">
                  🔧 Debug
                </Link>
                <Link to="/jwt-debug" className="nav-link debug-link">
                  🔑 JWT
                </Link>
              </>
            )}
          </nav>

          <div className="auth-section">
            {loading ? (
              <div className="loading-auth">🔄</div>
            ) : isAuth ? (
              <div className="user-menu" ref={userMenuRef}>
                <button
                  className={`user-button ${isAdmin ? "admin-user" : ""}`}
                  onClick={() => setShowUserMenu(!showUserMenu)}
                >
                  {isAdmin ? "👑" : "👤"}{" "}
                  {user?.firstName ||
                    user?.name ||
                    user?.username ||
                    "Kullanıcı"}
                  <span className="dropdown-arrow">▼</span>
                </button>

                {showUserMenu && (
                  <div className="user-dropdown">
                    <div className="user-info">
                      <div className="user-name">
                        {user?.firstName && user?.lastName
                          ? `${user.firstName} ${user.lastName}`
                          : user?.name || user?.username || "Kullanıcı"}
                      </div>
                      <div className="user-email">{user?.email || ""}</div>
                      {userRoles.length > 0 && (
                        <div className="user-roles">
                          {userRoles.map((role) => (
                            <span key={role} className={`role-badge ${role}`}>
                              {role === "admin" ? "👑" : "👤"} {role}
                            </span>
                          ))}
                        </div>
                      )}
                      <div className="user-login-time">
                        Giriş:{" "}
                        {user?.loginTime
                          ? new Date(user.loginTime).toLocaleTimeString()
                          : ""}
                      </div>
                    </div>
                    <hr />
                    <div className="user-actions">
                      <Link
                        to="/profile"
                        className="user-action-link"
                        onClick={() => setShowUserMenu(false)}
                      >
                        👤 Profil
                      </Link>
                      <Link
                        to="/settings"
                        className="user-action-link"
                        onClick={() => setShowUserMenu(false)}
                      >
                        ⚙️ Ayarlar
                      </Link>
                      <button onClick={handleLogout} className="logout-btn">
                        🚪 Çıkış Yap
                      </button>
                    </div>
                  </div>
                )}
              </div>
            ) : (
              <div className="auth-links">
                <Link to="/login" className="auth-link login">
                  🔑 Giriş
                </Link>
                <Link to="/register" className="auth-link register">
                  🎉 Kayıt Ol
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
