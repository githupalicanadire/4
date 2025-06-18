import api from "./api";

export const basketService = {
  // Get basket by username
  getBasket: async (userName) => {
    try {
      console.log(`🛒 Fetching basket for user: ${userName}`);
      const response = await api.get(`/basket-service/basket/${userName}`);

      // Backend GetBasketResponse formatında { cart: ShoppingCart } döner
      const basket = response.data.cart || response.data;

      // Backend'den gelen basket formatını normalize et
      return {
        userName: basket.userName || userName,
        items: basket.items || [],
        totalPrice: basket.totalPrice || 0,
      };
    } catch (error) {
      console.error("❌ Get basket error:", error);

      // Eğer basket bulunamazsa (404), boş basket döndür
      if (error.originalError?.response?.status === 404) {
        console.log("📦 Creating new empty basket");
        return {
          userName: userName,
          items: [],
          totalPrice: 0,
        };
      }

      throw error;
    }
  },

  // Store/Update basket
  storeBasket: async (basket) => {
    try {
      console.log("💾 Storing basket:", basket);

      // Backend'in beklediği StoreBasketRequest format
      const basketData = {
        cart: {
          userName: basket.userName,
          items: basket.items.map((item) => ({
            quantity: item.quantity,
            color: item.color || "Default",
            price: item.price,
            productId: item.productId,
            productName: item.productName,
          })),
        },
      };

      const response = await api.post("/basket-service/basket", basketData);
      return response.data;
    } catch (error) {
      console.error("❌ Store basket error:", error);
      throw error;
    }
  },

  // Delete basket
  deleteBasket: async (userName) => {
    try {
      console.log(`🗑️ Deleting basket for user: ${userName}`);
      const response = await api.delete(`/basket-service/basket/${userName}`);
      return response.data;
    } catch (error) {
      console.error("❌ Delete basket error:", error);
      throw error;
    }
  },

  // Checkout basket
  checkoutBasket: async (basketCheckout) => {
    try {
      console.log("🎯 Checking out basket:", basketCheckout);

      // Backend'in beklediği CheckoutBasketRequest formatı
      const checkoutData = {
        basketCheckoutDto: {
          userName: basketCheckout.userName,
          customerId:
            basketCheckout.customerId || "00000000-0000-0000-0000-000000000000", // Default GUID
          totalPrice: basketCheckout.totalPrice,

          // Shipping Address
          firstName: basketCheckout.firstName,
          lastName: basketCheckout.lastName,
          emailAddress: basketCheckout.emailAddress,
          addressLine: basketCheckout.addressLine,
          country: basketCheckout.country,
          state: basketCheckout.state,
          zipCode: basketCheckout.zipCode,

          // Payment
          cardName: basketCheckout.cardName,
          cardNumber: basketCheckout.cardNumber,
          expiration: basketCheckout.expiration,
          cvv: basketCheckout.cvv,
          paymentMethod: basketCheckout.paymentMethod || 1,
        },
      };

      const response = await api.post(
        "/basket-service/basket/checkout",
        checkoutData,
      );
      return response.data;
    } catch (error) {
      console.error("❌ Checkout basket error:", error);
      throw error;
    }
  },

  // Add item to basket
  addItemToBasket: async (userName, item) => {
    try {
      console.log(`➕ Adding item to basket for ${userName}:`, item);

      // Önce mevcut basket'i al
      const currentBasket = await basketService.getBasket(userName);

      // Item'ı basket'e ekle veya miktarını artır
      const existingItemIndex = currentBasket.items.findIndex(
        (basketItem) => basketItem.productId === item.productId,
      );

      if (existingItemIndex >= 0) {
        // Mevcut item'ın miktarını artır
        currentBasket.items[existingItemIndex].quantity += item.quantity || 1;
        currentBasket.items[existingItemIndex].price =
          currentBasket.items[existingItemIndex].quantity * item.unitPrice;
      } else {
        // Yeni item ekle
        currentBasket.items.push({
          productId: item.productId,
          productName: item.productName,
          quantity: item.quantity || 1,
          price: (item.quantity || 1) * item.unitPrice,
          color: item.color || "Default",
        });
      }

      // Toplam fiyatı hesapla
      currentBasket.totalPrice = currentBasket.items.reduce(
        (total, basketItem) => total + basketItem.price,
        0,
      );

      // Güncellenmiş basket'i kaydet
      return await basketService.storeBasket(currentBasket);
    } catch (error) {
      console.error("❌ Add item to basket error:", error);
      throw error;
    }
  },
};
