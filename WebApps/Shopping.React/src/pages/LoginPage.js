import React, { useState, useEffect } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import "./LoginPage.css";

const LoginPage = () => {
  const { login, isAuthenticated, loading } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoggingIn, setIsLoggingIn] = useState(false);

  // Redirect if already authenticated
  useEffect(() => {
    if (isAuthenticated() && !loading) {
      const from = location.state?.from?.pathname || "/products";
      console.log("🔄 Already authenticated, redirecting to:", from);
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, loading, navigate, location]);

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");
    setIsLoggingIn(true);

    try {
      const result = await login(username, password);
      if (result.success) {
        // Redirect to intended page or default
        const from = location.state?.from?.pathname || "/products";
        console.log("✅ Login successful, redirecting to:", from);
        navigate(from, { replace: true });
      } else {
        setError(result.message || "Giriş başarısız");
      }
    } catch (error) {
      console.error("❌ Login failed:", error);
      setError(
        error.message || "Giriş yapılırken hata oluştu. Lütfen tekrar deneyin.",
      );
    } finally {
      setIsLoggingIn(false);
    }
  };

  // Don't render if already authenticated
  if (isAuthenticated() && !loading) {
    return null;
  }

  return (
    <div className="login-page">
      <div className="login-container">
        <div className="login-header">
          <h1>🎮 ToyLand'e Giriş 🧸</h1>
          <p>Hesabınıza giriş yaparak alışverişe devam edin</p>
        </div>

        <form className="login-form" onSubmit={handleLogin}>
          {error && <div className="error-message">❌ {error}</div>}

          {/* Demo credentials info */}
          <div className="demo-credentials">
            <h4>🧪 Demo Hesaplar:</h4>
            <div className="demo-account">
              <strong>👑 Admin:</strong> admin / Admin123!
            </div>
            <div className="demo-account">
              <strong>👤 Müşteri:</strong> swn / Password123!
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="username">Kullanıcı Adı</label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              disabled={isLoggingIn}
              placeholder="admin veya swn"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Şifre</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={isLoggingIn}
              placeholder="Admin123! veya Password123!"
              required
            />
          </div>

          <button
            type="submit"
            className={`login-btn ${isLoggingIn ? "loading" : ""}`}
            disabled={isLoggingIn}
          >
            {isLoggingIn ? "🔄 Giriş Yapılıyor..." : "🚀 Giriş Yap"}
          </button>
        </form>

        <div className="login-footer">
          <p>
            Hesabınız yok mu?{" "}
            <Link to="/register" state={{ from: location.state?.from }}>
              🎉 Ücretsiz Kayıt Ol
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
