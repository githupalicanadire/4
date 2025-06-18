// Legacy demo user configuration - kept for debugging purposes
export const USER_CONFIG = {
  // Demo users for debugging
  DEMO_USERS: {
    swn: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    admin: "3fa85f64-5717-4562-b3fc-2c963f66afa0",
  },
};

// Note: These functions are now deprecated in favor of AuthContext
// They're kept for backward compatibility and debugging

// Helper function to get demo customer ID for debugging
export const getDemoCustomerId = (username) => {
  return (
    USER_CONFIG.DEMO_USERS[username] || "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  );
};

// Helper function to get all available demo users for debugging
export const getAvailableDemoUsers = () => {
  return Object.keys(USER_CONFIG.DEMO_USERS);
};

// DEPRECATED: Use AuthContext instead
export const getCurrentUser = () => {
  console.warn("getCurrentUser is deprecated. Use AuthContext instead.");
  return "guest";
};

// DEPRECATED: Use AuthContext instead
export const getCurrentCustomerId = () => {
  console.warn("getCurrentCustomerId is deprecated. Use AuthContext instead.");
  return null;
};

// DEPRECATED: Use AuthContext instead
export const switchDemoUser = (username) => {
  console.warn(
    "switchDemoUser is deprecated. Use proper authentication instead.",
  );
  return false;
};
