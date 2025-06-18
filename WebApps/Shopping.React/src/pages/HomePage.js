import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import "./HomePage.css";

const HomePage = () => {
  const { isAuthenticated, getCurrentUser } = useAuth();
  const isAuth = isAuthenticated();
  const userName = getCurrentUser();

  return (
    <div className="home-page">
      <div className="hero-section">
        <div className="hero-content">
          <h1>🎪 ToyLand'e Hoş Geldiniz! 🎠</h1>
          {isAuth ? (
            <>
              <p>🎉 Hoş geldin, {userName}! 🌟</p>
              <p>
                🛒 Sepetine ürün ekleyebilir ve siparişlerini takip edebilirsin!
              </p>
              <div className="auth-actions">
                <Link to="/products" className="btn btn-primary hero-btn">
                  🧸 Oyuncakları Keşfet 🎮
                </Link>
                <Link to="/cart" className="btn btn-secondary hero-btn">
                  🛒 Sepetim
                </Link>
              </div>
            </>
          ) : (
            <>
              <p>
                🌟 Çocukların hayal dünyasını renklendiren en eğlenceli
                oyuncaklar burada! 🎨
              </p>
              <p>🔑 Sepete ürün eklemek ve sipariş vermek için giriş yapın!</p>
              <div className="auth-actions">
                <Link to="/products" className="btn btn-secondary hero-btn">
                  🧸 Oyuncakları İncele 👀
                </Link>
                <Link to="/login" className="btn btn-primary hero-btn">
                  🔑 Giriş Yap & Alışveriş Et 🛒
                </Link>
              </div>
            </>
          )}
        </div>
      </div>

      <div className="features-section">
        <div className="container">
          <h2>🎯 Neden ToyLand? 🌈</h2>
          <div className="features-grid">
            <div className="feature-card">
              <h3>🧩 Eğitici Oyuncaklar</h3>
              <p>
                Çocukların gelişimini destekleyen, öğretici ve eğlenceli
                oyuncaklar
              </p>
            </div>
            <div className="feature-card">
              <h3>🎨 Yaratıcılık</h3>
              <p>Hayal gücünü geliştiren sanat ve zanaat malzemeleri</p>
            </div>
            <div className="feature-card">
              <h3>🤖 Teknoloji Oyuncakları</h3>
              <p>
                Geleceğin teknolojilerini öğreten robotlar ve kodlama
                oyuncakları
              </p>
            </div>
            <div className="feature-card">
              <h3>🚗 Klasik Oyuncaklar</h3>
              <p>Nesiller boyu sevilen klasik oyuncaklar ve koleksiyonlar</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
