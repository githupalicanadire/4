import React, { useState, useEffect } from "react";
import { useAuth } from "../contexts/AuthContext";

const AuthDebugPage = () => {
  const { user, isAuthenticated, getAccessToken, getCurrentUser } = useAuth();
  const [localStorageData, setLocalStorageData] = useState({});
  const [decodedToken, setDecodedToken] = useState(null);

  const decodeJWT = (token) => {
    if (!token) return null;
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
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error("Error decoding JWT:", error);
      return null;
    }
  };

  useEffect(() => {
    // Get all auth-related localStorage data
    const authData = {
      access_token: localStorage.getItem("access_token"),
      user: localStorage.getItem("user"),
      refresh_token: localStorage.getItem("refresh_token"),
    };
    setLocalStorageData(authData);

    // Decode the access token
    if (authData.access_token) {
      const decoded = decodeJWT(authData.access_token);
      setDecodedToken(decoded);
    }
  }, []);

  const clearAuth = () => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("user");
    localStorage.removeItem("refresh_token");
    window.location.reload();
  };

  const testBasketAPI = async () => {
    try {
      const response = await fetch("/api/basket-service/basket", {
        headers: {
          Authorization: `Bearer ${getAccessToken()}`,
          "Content-Type": "application/json",
        },
      });

      console.log("Basket API Response:", response.status, response.statusText);

      if (!response.ok) {
        const errorText = await response.text();
        console.error("Basket API Error:", errorText);
        alert(`Basket API Error: ${response.status} - ${errorText}`);
      } else {
        const data = await response.json();
        console.log("Basket API Success:", data);
        alert("Basket API Success! Check console for details.");
      }
    } catch (error) {
      console.error("Basket API Request Error:", error);
      alert(`Request Error: ${error.message}`);
    }
  };

  return (
    <div className="container mt-4">
      <h2>🔍 Authentication Debug</h2>

      <div className="row">
        <div className="col-md-6">
          <div className="card mb-3">
            <div className="card-header">
              <h5>🔐 Auth State</h5>
            </div>
            <div className="card-body">
              <table className="table table-sm">
                <tbody>
                  <tr>
                    <td>
                      <strong>Is Authenticated:</strong>
                    </td>
                    <td>{isAuthenticated() ? "✅ Yes" : "❌ No"}</td>
                  </tr>
                  <tr>
                    <td>
                      <strong>Current User:</strong>
                    </td>
                    <td>{getCurrentUser() || "Not logged in"}</td>
                  </tr>
                  <tr>
                    <td>
                      <strong>Has User Object:</strong>
                    </td>
                    <td>{user ? "✅ Yes" : "❌ No"}</td>
                  </tr>
                  <tr>
                    <td>
                      <strong>Has Access Token:</strong>
                    </td>
                    <td>{getAccessToken() ? "✅ Yes" : "❌ No"}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <div className="card mb-3">
            <div className="card-header">
              <h5>💾 LocalStorage Data</h5>
            </div>
            <div className="card-body">
              <div className="mb-2">
                <strong>access_token:</strong>
                <div style={{ fontSize: "12px", wordBreak: "break-all" }}>
                  {localStorageData.access_token
                    ? localStorageData.access_token.substring(0, 100) + "..."
                    : "❌ Not found"}
                </div>
              </div>
              <div className="mb-2">
                <strong>user:</strong>
                <div style={{ fontSize: "12px", wordBreak: "break-all" }}>
                  {localStorageData.user
                    ? localStorageData.user.substring(0, 200) + "..."
                    : "❌ Not found"}
                </div>
              </div>
              <div className="mb-2">
                <strong>refresh_token:</strong>
                <div style={{ fontSize: "12px", wordBreak: "break-all" }}>
                  {localStorageData.refresh_token
                    ? localStorageData.refresh_token.substring(0, 100) + "..."
                    : "❌ Not found"}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="col-md-6">
          <div className="card mb-3">
            <div className="card-header">
              <h5>👤 User Details</h5>
            </div>
            <div className="card-body">
              {user ? (
                <table className="table table-sm">
                  <tbody>
                    <tr>
                      <td>
                        <strong>Username:</strong>
                      </td>
                      <td>{user.username}</td>
                    </tr>
                    <tr>
                      <td>
                        <strong>Name:</strong>
                      </td>
                      <td>{user.name}</td>
                    </tr>
                    <tr>
                      <td>
                        <strong>Email:</strong>
                      </td>
                      <td>{user.email}</td>
                    </tr>
                    <tr>
                      <td>
                        <strong>Roles:</strong>
                      </td>
                      <td>{user.roles?.join(", ") || "None"}</td>
                    </tr>
                    <tr>
                      <td>
                        <strong>Subject (sub):</strong>
                      </td>
                      <td>{user.sub}</td>
                    </tr>
                    <tr>
                      <td>
                        <strong>Token Expires:</strong>
                      </td>
                      <td>
                        {user.expires_at
                          ? new Date(user.expires_at).toLocaleString()
                          : "Unknown"}
                      </td>
                    </tr>
                  </tbody>
                </table>
              ) : (
                <p>No user data available</p>
              )}
            </div>
          </div>

          <div className="card mb-3">
            <div className="card-header">
              <h5>🧪 API Tests</h5>
            </div>
            <div className="card-body">
              <button
                className="btn btn-primary mb-2"
                onClick={testBasketAPI}
                disabled={!getAccessToken()}
              >
                Test Basket API
              </button>
              <br />
              <button className="btn btn-danger" onClick={clearAuth}>
                Clear Auth Data
              </button>
            </div>
          </div>
        </div>
      </div>

      {getAccessToken() && (
        <div className="card">
          <div className="card-header">
            <h5>🔑 JWT Token Details</h5>
          </div>
          <div className="card-body">
            <div className="mb-2">
              <strong>Full Token:</strong>
              <textarea
                className="form-control"
                rows="3"
                readOnly
                value={getAccessToken()}
                style={{ fontSize: "12px" }}
              />
            </div>
            {(() => {
              try {
                const token = getAccessToken();
                const payload = JSON.parse(atob(token.split(".")[1]));
                return (
                  <div>
                    <strong>Decoded Payload:</strong>
                    <pre
                      style={{
                        fontSize: "12px",
                        backgroundColor: "#f8f9fa",
                        padding: "10px",
                      }}
                    >
                      {JSON.stringify(payload, null, 2)}
                    </pre>
                  </div>
                );
              } catch (error) {
                return <div>Error decoding token: {error.message}</div>;
              }
            })()}
          </div>
        </div>
      )}
    </div>
  );
};

export default AuthDebugPage;
