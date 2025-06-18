import api from "./api";

export const orderingService = {
  // Get orders with pagination
  getOrders: async (pageNumber = 1, pageSize = 10) => {
    try {
      console.log(`📋 Fetching orders: page ${pageNumber}, size ${pageSize}`);
      const response = await api.get("/ordering-service/orders", {
        params: { pageNumber, pageSize },
      });

      console.log("📦 Orders response:", response.data);

      // Backend format normalize et
      const orders =
        response.data.orders || response.data.data || response.data || [];

      return {
        data: Array.isArray(orders) ? orders : [],
        pageNumber: response.data.pageNumber || pageNumber,
        pageSize: response.data.pageSize || pageSize,
        totalCount:
          response.data.totalCount ||
          (Array.isArray(orders) ? orders.length : 0),
      };
    } catch (error) {
      console.error("❌ Get orders error:", error);
      throw error;
    }
  },

  // Get orders by customer
  getOrdersByCustomer: async (customerId) => {
    try {
      console.log(`👤 Fetching orders for customer: ${customerId}`);
      const response = await api.get(
        `/ordering-service/orders/customer/${customerId}`,
      );

      const orders =
        response.data.orders || response.data.data || response.data || [];

      return {
        data: Array.isArray(orders) ? orders : [],
      };
    } catch (error) {
      console.error("❌ Get customer orders error:", error);
      throw error;
    }
  },

  // Get orders by name
  getOrdersByName: async (name) => {
    try {
      console.log(`🔍 Fetching orders by name: ${name}`);
      const response = await api.get(
        `/ordering-service/orders/name/${encodeURIComponent(name)}`,
      );

      const orders =
        response.data.orders || response.data.data || response.data || [];

      return {
        data: Array.isArray(orders) ? orders : [],
      };
    } catch (error) {
      console.error("❌ Get orders by name error:", error);
      throw error;
    }
  },

  // Create order (This is typically handled by basket checkout)
  createOrder: async (orderData) => {
    try {
      console.log("📝 Creating order:", orderData);

      // Backend'in beklediği CreateOrderCommand formatı
      const createOrderCommand = {
        customerId:
          orderData.customerId || "00000000-0000-0000-0000-000000000000",
        orderName: orderData.orderName || `Order-${Date.now()}`,

        // Shipping Address
        shippingAddress: {
          firstName: orderData.firstName,
          lastName: orderData.lastName,
          emailAddress: orderData.emailAddress,
          addressLine: orderData.addressLine,
          country: orderData.country,
          state: orderData.state,
          zipCode: orderData.zipCode,
        },

        // Billing Address (aynı shipping address kullan)
        billingAddress: {
          firstName: orderData.firstName,
          lastName: orderData.lastName,
          emailAddress: orderData.emailAddress,
          addressLine: orderData.addressLine,
          country: orderData.country,
          state: orderData.state,
          zipCode: orderData.zipCode,
        },

        // Payment
        payment: {
          cardName: orderData.cardName,
          cardNumber: orderData.cardNumber,
          expiration: orderData.expiration,
          cvv: orderData.cvv,
          paymentMethod: orderData.paymentMethod || 1,
        },

        // Order Items
        orderItems: orderData.orderItems || [],
      };

      const response = await api.post(
        "/ordering-service/orders",
        createOrderCommand,
      );
      return response.data;
    } catch (error) {
      console.error("❌ Create order error:", error);
      throw error;
    }
  },

  // Update order
  updateOrder: async (order) => {
    try {
      console.log("📝 Updating order:", order);
      const response = await api.put("/ordering-service/orders", order);
      return response.data;
    } catch (error) {
      console.error("❌ Update order error:", error);
      throw error;
    }
  },

  // Delete order
  deleteOrder: async (id) => {
    try {
      console.log(`🗑️ Deleting order: ${id}`);
      const response = await api.delete(`/ordering-service/orders/${id}`);
      return response.data;
    } catch (error) {
      console.error("❌ Delete order error:", error);
      throw error;
    }
  },

  // Get order statuses for dropdown/filter
  getOrderStatuses: () => {
    return [
      { value: 0, label: "Beklemede", color: "#ffc107" },
      { value: 1, label: "İşleniyor", color: "#17a2b8" },
      { value: 2, label: "Kargoda", color: "#007bff" },
      { value: 3, label: "Teslim Edildi", color: "#28a745" },
      { value: 4, label: "İptal Edildi", color: "#dc3545" },
    ];
  },
};
