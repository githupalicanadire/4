import React, { useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import "./RegisterPage.css";

const RegisterPage = () => {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const { register } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
    // Clear messages when user starts typing
    if (error) setError("");
    if (success) setSuccess("");
  };

  const validateForm = () => {
    if (formData.password !== formData.confirmPassword) {
      setError("Şifreler eşleşmiyor");
      return false;
    }

    if (formData.password.length < 6) {
      setError("Şifre en az 6 karakter olmalıdır");
      return false;
    }

    if (!formData.email.includes("@")) {
      setError("Geçerli bir email adresi girin");
      return false;
    }

    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) return;

    setLoading(true);
    setError("");
    setSuccess("");

    const result = await register(formData);

    if (result.success) {
      setSuccess(result.message);
      // Redirect to login after 2 seconds
      setTimeout(() => {
        navigate("/login", {
          state: { from: location.state?.from },
          replace: true,
        });
      }, 2000);
    } else {
      setError(result.message);
    }

    setLoading(false);
  };

  return (
    <div className="register-page">
      <div className="register-container">
        <div className="register-header">
          <h1>🎉 ToyLand'e Katıl! 🧸</h1>
          <p>Ücretsiz hesap oluşturun ve alışverişe başlayın</p>
        </div>

        <form onSubmit={handleSubmit} className="register-form">
          {error && <div className="error-message">⚠️ {error}</div>}

          {success && <div className="success-message">✅ {success}</div>}

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="firstName">👤 Ad</label>
              <input
                type="text"
                id="firstName"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                required
                placeholder="Adınız"
                disabled={loading}
              />
            </div>

            <div className="form-group">
              <label htmlFor="lastName">👤 Soyad</label>
              <input
                type="text"
                id="lastName"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                required
                placeholder="Soyadınız"
                disabled={loading}
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="username">🎮 Kullanıcı Adı</label>
            <input
              type="text"
              id="username"
              name="username"
              value={formData.username}
              onChange={handleChange}
              required
              placeholder="Benzersiz kullanıcı adı seçin"
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="email">📧 Email</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              placeholder="email@example.com"
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">🔒 Şifre</label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
              placeholder="En az 6 karakter"
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">🔒 Şifre Tekrar</label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              placeholder="Şifrenizi tekrar girin"
              disabled={loading}
            />
          </div>

          <button
            type="submit"
            className="register-btn"
            disabled={
              loading ||
              !formData.firstName ||
              !formData.lastName ||
              !formData.username ||
              !formData.email ||
              !formData.password ||
              !formData.confirmPassword
            }
          >
            {loading ? "🔄 Hesap oluşturuluyor..." : "🚀 Hesap Oluştur"}
          </button>
        </form>

        <div className="register-footer">
          <p>
            Zaten hesabınız var mı?{" "}
            <Link to="/login" state={{ from: location.state?.from }}>
              🎮 Giriş Yap
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
