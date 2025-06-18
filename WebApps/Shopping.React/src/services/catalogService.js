import api from "./api";

export const catalogService = {
  // Get all products with pagination
  getProducts: async (pageNumber = 1, pageSize = 12) => {
    try {
      console.log(`🛍️ Fetching products: page ${pageNumber}, size ${pageSize}`);
      const response = await api.get(`/catalog-service/products`, {
        params: { pageNumber, pageSize },
      });

      console.log("📦 Products response:", response.data);

      // Backend format: { "products": [...] } or direct array
      const products =
        response.data.products || response.data.data || response.data;

      return {
        data: Array.isArray(products) ? products : [],
        pageNumber: response.data.pageNumber || pageNumber,
        pageSize: response.data.pageSize || pageSize,
        totalCount:
          response.data.totalCount ||
          (Array.isArray(products) ? products.length : 0),
      };
    } catch (error) {
      console.error("❌ Catalog service error:", error);
      throw error;
    }
  },

  // Get product by ID
  getProductById: async (id) => {
    try {
      console.log(`🔍 Fetching product by ID: ${id}`);
      const response = await api.get(`/catalog-service/products/${id}`);

      // Backend format: { "product": {...} } or direct object
      const product = response.data.product || response.data;

      return { product };
    } catch (error) {
      console.error("❌ Get product by ID error:", error);
      throw error;
    }
  },

  // Get products by category
  getProductsByCategory: async (category) => {
    try {
      console.log(`🏷️ Fetching products by category: ${category}`);
      const response = await api.get(
        `/catalog-service/products/category/${encodeURIComponent(category)}`,
      );

      // Backend format: { "products": [...] } or direct array
      const products =
        response.data.products || response.data.data || response.data;

      return {
        data: Array.isArray(products) ? products : [],
        category: category,
      };
    } catch (error) {
      console.error("❌ Get products by category error:", error);
      throw error;
    }
  },

  // Get all available categories
  getCategories: async () => {
    try {
      console.log("📋 Fetching categories");

      // Backend'de özel category endpoint'i yoksa, tüm ürünlerden kategorileri çıkaralım
      const response = await api.get(`/catalog-service/products`, {
        params: { pageNumber: 1, pageSize: 1000 }, // Tüm ürünleri al
      });

      const products =
        response.data.products || response.data.data || response.data || [];

      // Tüm kategorileri topla ve unique yap
      const allCategories = products.reduce((categories, product) => {
        if (product.category && Array.isArray(product.category)) {
          return [...categories, ...product.category];
        }
        return categories;
      }, []);

      const uniqueCategories = [...new Set(allCategories)];

      return { data: uniqueCategories };
    } catch (error) {
      console.error("❌ Get categories error:", error);
      // Default categories eğer API'den alamazsak
      return {
        data: [
          "Kutu Oyunları",
          "Müzik Aletleri",
          "Eğitici Oyuncaklar",
          "Peluş Oyuncaklar",
          "Oyuncak Arabalar",
          "Yapım Oyuncakları",
          "Oyuncak Bebekler ve Aksesuarları",
        ],
      };
    }
  },
};
