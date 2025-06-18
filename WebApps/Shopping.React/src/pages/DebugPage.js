import React, { useState } from "react";
import {
  getCurrentUser,
  getCurrentCustomerId,
  switchDemoUser,
  getAvailableDemoUsers,
} from "../config/userConfig";
import { basketService } from "../services/basketService";
import "./DebugPage.css";

const DebugPage = () => {
  const [currentUser, setCurrentUser] = useState(getCurrentUser());
  const [basketData, setBasketData] = useState(null);
  const [loading, setLoading] = useState(false);

  const availableUsers = getAvailableDemoUsers();

  const handleUserSwitch = async (username) => {
    if (switchDemoUser(username)) {
      setCurrentUser(username);
      setBasketData(null); // Clear previous basket data

      // Automatically fetch basket for new user
      await fetchUserBasket(username);
    }
  };

  const fetchUserBasket = async (username = currentUser) => {
    try {
      setLoading(true);
      const basket = await basketService.getBasket(username);
      setBasketData(basket);
    } catch (error) {
      console.error("Error fetching basket:", error);
      setBasketData({ error: error.message });
    } finally {
      setLoading(false);
    }
  };

  const clearUserBasket = async () => {
    try {
      setLoading(true);
      await basketService.deleteBasket(currentUser);
      setBasketData({ userName: currentUser, items: [], totalPrice: 0 });
    } catch (error) {
      console.error("Error clearing basket:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="debug-page">
      <div className="debug-header">
        <h1>🔧 Debug Panel</h1>
        <p>Test different users and their baskets</p>
      </div>

      <div className="debug-content">
        <div className="user-switcher">
          <h3>👤 Current User</h3>
          <div className="current-user-info">
            <strong>Username:</strong> {currentUser}
            <br />
            <strong>Customer ID:</strong> {getCurrentCustomerId()}
          </div>

          <h4>Switch to Different User:</h4>
          <div className="user-buttons">
            {availableUsers.map((username) => (
              <button
                key={username}
                className={`user-btn ${currentUser === username ? "active" : ""}`}
                onClick={() => handleUserSwitch(username)}
              >
                {username}
              </button>
            ))}
          </div>
        </div>

        <div className="basket-debug">
          <h3>🛒 Basket Debug</h3>
          <div className="basket-actions">
            <button onClick={() => fetchUserBasket()} disabled={loading}>
              {loading ? "Loading..." : "Fetch Basket"}
            </button>
            <button onClick={clearUserBasket} disabled={loading}>
              Clear Basket
            </button>
          </div>

          {basketData && (
            <div className="basket-data">
              <h4>Basket Data:</h4>
              <pre>{JSON.stringify(basketData, null, 2)}</pre>
            </div>
          )}
        </div>

        <div className="api-info">
          <h3>🌐 API Information</h3>
          <div className="api-endpoints">
            <h4>Current API Endpoints:</h4>
            <ul>
              <li>
                Identity Service: <code>http://localhost:6007</code>
              </li>
              <li>
                Catalog Service: <code>http://localhost:6000</code>
              </li>
              <li>
                Basket Service: <code>http://localhost:6001</code>
              </li>
              <li>
                Ordering Service: <code>http://localhost:6003</code>
              </li>
              <li>
                API Gateway: <code>http://localhost:6004</code>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DebugPage;
