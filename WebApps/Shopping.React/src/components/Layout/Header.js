import React, { useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import "./Header.css";

const Header = () => {
  const { user, isAuthenticated, logout } = useAuth();
  const [showUserMenu, setShowUserMenu] = useState(false);

  // Debug: Check authentication state
  const authState = isAuthenticated();
  console.log("🏠 Header: Auth state -", {
    hasUser: !!user,
    isAuth: authState,
    userName: user?.username || user?.name,
    firstName: user?.firstName,
    roles: user?.roles,
  });

  const handleLogout = () => {
    logout();
    setShowUserMenu(false);
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

            {isAuthenticated() ? (
              <>
                <Link to="/cart" className="nav-link">
                  🛒 Sepetim
                </Link>
                <Link to="/orders" className="nav-link">
                  📦 Siparişlerim
                </Link>
              </>
            ) : null}

            {process.env.NODE_ENV === "development" && (
              <>
                <Link to="/debug" className="nav-link debug-link">
                  🔧 Debug
                </Link>
                <Link to="/jwt-debug" className="nav-link debug-link">
                  🔑 JWT Debug
                </Link>
              </>
            )}
          </nav>

          <div className="auth-section">
            {isAuthenticated() ? (
              <div className="user-menu">
                <button
                  className="user-button"
                  onClick={() => setShowUserMenu(!showUserMenu)}
                >
                  👤{" "}
                  {user?.firstName ||
                    user?.name ||
                    user?.username ||
                    "Kullanıcı"}
                </button>

                {showUserMenu && (
                  <div className="user-dropdown">
                    <div className="user-info">
                      <strong>
                        {user?.firstName && user?.lastName
                          ? `${user.firstName} ${user.lastName}`
                          : user?.name || user?.username || "Kullanıcı"}
                      </strong>
                      <span>{user?.email || "Email bulunamadı"}</span>
                      {user?.roles && user.roles.length > 0 && (
                        <small>👑 {user.roles.join(", ")}</small>
                      )}
                    </div>
                    <hr />
                    <button onClick={handleLogout} className="logout-btn">
                      🚪 Çıkış Yap
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <div className="auth-links">
                <Link to="/login" className="auth-link">
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
