import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "./ConfirmationPage.css";

const ConfirmationPage = () => {
  const [orderData, setOrderData] = useState(null);

  useEffect(() => {
    // Get order data from session storage
    const lastOrder = sessionStorage.getItem("lastOrder");
    if (lastOrder) {
      setOrderData(JSON.parse(lastOrder));
      // Clear the order data after displaying
      sessionStorage.removeItem("lastOrder");
    }
  }, []);
  return (
    <div className="confirmation-page">
      <div className="confirmation-card">
        <div className="success-icon celebration">
          <div className="icon-wrapper">
            <svg
              width="80"
              height="80"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <circle cx="12" cy="12" r="12" fill="#28a745" />
              <path
                d="M9 12l2 2 4-4"
                stroke="white"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </div>
          <div className="celebration-emojis">🎉🎊🥳🎈</div>
        </div>

        <h1>🎉 Oyuncağınız Sipariş Edildi! 🎁</h1>
        <p className="confirmation-message">
          🚀 Harika! Oyuncak siparişiniz başarıyla alındı ve hazırlanıyor. 🎊
          Oyuncağınızın durumunu e-posta adresinizden takip edebilirsiniz.
        </p>

        <div className="confirmation-details">
          <div className="detail-item">
            <strong>🎫 Sipariş Numarası:</strong>
            <span>
              #
              {orderData?.orderNumber ||
                "TOY-" + Math.random().toString(36).substr(2, 9).toUpperCase()}
            </span>
          </div>
          <div className="detail-item">
            <strong>📅 Sipariş Tarihi:</strong>
            <span>
              {new Date().toLocaleDateString("tr-TR", {
                year: "numeric",
                month: "long",
                day: "numeric",
                hour: "2-digit",
                minute: "2-digit",
              })}
            </span>
          </div>
          <div className="detail-item">
            <strong>👤 Müşteri:</strong>
            <span>{orderData?.customerName || "Değerli Müşterimiz"}</span>
          </div>
          <div className="detail-item">
            <strong>🎁 Toplam Ürün:</strong>
            <span>{orderData?.itemCount || 0} adet</span>
          </div>
          <div className="detail-item">
            <strong>💰 Toplam Tutar:</strong>
            <span>${orderData?.totalPrice?.toFixed(2) || "0.00"}</span>
          </div>
          <div className="detail-item">
            <strong>🚚 Tahmini Teslimat:</strong>
            <span>
              {new Date(
                Date.now() + 7 * 24 * 60 * 60 * 1000,
              ).toLocaleDateString("tr-TR", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })}
            </span>
          </div>
        </div>

        <div className="confirmation-actions">
          <Link to="/orders" className="btn btn-primary">
            📦 Siparişlerimi Görüntüle
          </Link>
          <Link to="/products" className="btn btn-secondary">
            🧸 Alışverişe Devam Et 🎮
          </Link>
        </div>
      </div>
    </div>
  );
};

export default ConfirmationPage;
