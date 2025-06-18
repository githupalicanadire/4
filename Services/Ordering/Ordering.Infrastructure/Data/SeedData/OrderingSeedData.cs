using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;
using Ordering.Domain.Enums;

namespace Ordering.Infrastructure.Data.SeedData;

public static class OrderingSeedData
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        if (!await context.Orders.AnyAsync())
        {
            logger.LogInformation("🌱 Seeding orders...");
            await SeedOrders(context, logger);
            logger.LogInformation("✅ Orders seeding completed");
        }
    }

    private static async Task SeedOrders(ApplicationDbContext context, ILogger logger)
    {
        var orders = new List<Order>();

        // Sample customers - using generated GUIDs (no hardcoded values)
        var customers = new[]
        {
            new { UserName = "admin", CustomerId = Guid.NewGuid() },
            new { UserName = "swn", CustomerId = Guid.NewGuid() },
            new { UserName = "alice", CustomerId = Guid.NewGuid() },
            new { UserName = "bob", CustomerId = Guid.NewGuid() },
            new { UserName = "emily", CustomerId = Guid.NewGuid() },
            new { UserName = "michael", CustomerId = Guid.NewGuid() },
            new { UserName = "sarah", CustomerId = Guid.NewGuid() },
            new { UserName = "david", CustomerId = Guid.NewGuid() },
            new { UserName = "lisa", CustomerId = Guid.NewGuid() },
            new { UserName = "john", CustomerId = Guid.NewGuid() },
            new { UserName = "maria", CustomerId = Guid.NewGuid() }
        };

        // Sample products (matching Catalog service toy products exactly)
        var sampleProducts = new[]
        {
            new { Id = Guid.Parse("5334c996-8457-4cf0-815c-ed2b77c4ff61"), Name = "Squid Game 5 Taş Oyunu", Price = 149.99m },
            new { Id = Guid.Parse("c67d6323-e8b0-4bdd-a7e6-a593eb6068e8"), Name = "Smile Games Günün Sorusu Kutu Oyunu", Price = 274.99m },
            new { Id = Guid.Parse("4f136e07-cc90-4d8b-b847-68c0a3331d79"), Name = "Smile Games Matematik Oyunu", Price = 589.99m },
            new { Id = Guid.Parse("6ec1297b-ec0a-4aa1-be25-6726e3b51a27"), Name = "UNO Reverse Pack Eklenti Paketi", Price = 119.99m },
            new { Id = Guid.Parse("b786103d-c621-4f5a-b498-23312a3fa792"), Name = "UNO Stack Pack Eklenti Paketi", Price = 119.99m },
            new { Id = Guid.Parse("c4bbc4a2-4555-45d8-97cc-2a99b2167bff"), Name = "UNO Teams Kartlar Kart Oyunu", Price = 319.99m },
            new { Id = Guid.Parse("9c8d7e6f-5a4b-3c2d-1e0f-9a8b7c6d5e4f"), Name = "Bontempi Işıklı Mikrofonlu Elektronik Tabureli Org", Price = 2699.00m },
            new { Id = Guid.Parse("ad9e8f7a-6b5c-4d3e-2f1a-0b9c8d7e6f5a"), Name = "Bontempi Ayaklı Mikrofonlu Pembe Elektronik Çocuk Orgu", Price = 1429.00m },
            new { Id = Guid.Parse("f24d3e2f-1a0b-9c8d-7e6f-5a4b3c2d1e0f"), Name = "Fisher Price Matematikçi Timsah", Price = 1399.99m },
            new { Id = Guid.Parse("2570a6b5-4c3d-2e1f-0a9b-8c7d6e5f4a3b"), Name = "Sesli Disney Stitch Real Fx Elektronik Kukla 30 cm", Price = 4289.99m },
            new { Id = Guid.Parse("58a3d9e8-7f6a-5b4c-3d2e-1f0a9b8c7d6e"), Name = "1:64 Hot Wheels The Hot Ones Fiat 500 Topolino (1936)", Price = 249.99m },
            new { Id = Guid.Parse("69b4eaf9-8a7b-6c5d-4e3f-2a1b0c9d8e7f"), Name = "1:24 Lamborghini Sian FKP 37 Model Araba (Yeşil)", Price = 1199.00m },
            new { Id = Guid.Parse("7ac5fb0a-9b8c-7d6e-5f4a-3b2c1d0e9f8a"), Name = "1:12 USB Şarjlı Uzaktan Kumandalı 4x4 Arazi Aracı", Price = 3699.00m },
            new { Id = Guid.Parse("8bd60c1b-ac9d-8e7f-6a5b-4c3d2e1f0a9b"), Name = "LEGO Disney Walt Disney Hatırası Kamera", Price = 3699.00m },
            new { Id = Guid.Parse("9ce71d2c-bd0e-9f8a-7b6c-5d4e3f2a1b0c"), Name = "LEGO Marvel X-Men: X-Mansion", Price = 12999.00m },
            new { Id = Guid.Parse("adf82e3d-ce1f-0a9b-8c7d-6e5f4a3b2c1d"), Name = "LEGO Technic Ferrari SF-24 F1 Araba", Price = 9399.00m }
        };

        var random = new Random(42); // Fixed seed for consistent results
        var orderCounter = 1;

        // Create special admin order with Lamborghini
        var adminCustomer = customers.First(c => c.UserName == "admin");
        var adminOrderId = OrderId.Of(Guid.NewGuid());
        var adminCustomerId = CustomerId.Of(adminCustomer.CustomerId);
        var adminOrderName = OrderName.Of($"ORD-ADMIN-001");

        var adminAddress = Address.Of("Admin", "User", "admin@shopping.com", "İstanbul Teknoloji Merkezi, Maslak", "Turkey", "Istanbul", "34485");
        var adminPayment = Payment.Of("Admin User", "5555-****-****-4444", "12/25", "***", 1);

        var adminOrder = Order.Create(
            adminOrderId,
            adminCustomerId,
            adminOrderName,
            adminAddress,
            adminAddress,
            adminPayment
        );

        // Add Lamborghini to admin order
        var lamborghiniProduct = sampleProducts.First(p => p.Name.Contains("Lamborghini"));
        adminOrder.Add(ProductId.Of(lamborghiniProduct.Id), 1, lamborghiniProduct.Price);

        // Set order as completed and created 7 days ago
        var statusProperty = typeof(Order).GetProperty(nameof(Order.Status));
        statusProperty?.SetValue(adminOrder, OrderStatus.Completed);

        var createdAtProperty = typeof(Order).GetProperty("CreatedAt") ?? typeof(Order).GetProperty("Created");
        if (createdAtProperty != null && createdAtProperty.CanWrite)
        {
            createdAtProperty.SetValue(adminOrder, DateTime.UtcNow.AddDays(-7));
        }

        orders.Add(adminOrder);
        orderCounter++;

        foreach (var customer in customers.Where(c => c.UserName != "admin"))
        {
            // Each customer gets 1-3 orders
            var orderCount = random.Next(1, 4);

            for (int i = 0; i < orderCount; i++)
            {
                var orderId = OrderId.Of(Guid.NewGuid());
                var customerId = CustomerId.Of(customer.CustomerId);
                var orderName = OrderName.Of($"ORD-{orderCounter:D6}");

                // More realistic Turkish addresses
                var addresses = new[]
                {
                    Address.Of($"{customer.UserName}", "Müşteri", $"{customer.UserName}@shopping.com", "Bağdat Caddesi No: 123 Kadıköy", "Turkey", "Istanbul", "34710"),
                    Address.Of($"{customer.UserName}", "Müşteri", $"{customer.UserName}@shopping.com", "Tunalı Hilmi Caddesi No: 45 Çankaya", "Turkey", "Ankara", "06680"),
                    Address.Of($"{customer.UserName}", "Müşteri", $"{customer.UserName}@shopping.com", "Alsancak Mahallesi 1482 Sokak No: 12", "Turkey", "Izmir", "35220"),
                    Address.Of($"{customer.UserName}", "Müşteri", $"{customer.UserName}@shopping.com", "Osmangazi Mahallesi Çekirge Caddesi No: 67", "Turkey", "Bursa", "16080")
                };
                var shippingAddress = addresses[random.Next(addresses.Length)];

                // Realistic payment info (masked card numbers)
                var payments = new[]
                {
                    Payment.Of($"{customer.UserName}", "4532-****-****-1234", "12/26", "***", 1),
                    Payment.Of($"{customer.UserName}", "5461-****-****-5678", "08/27", "***", 1),
                    Payment.Of($"{customer.UserName}", "4169-****-****-9012", "03/28", "***", 1)
                };
                var payment = payments[random.Next(payments.Length)];

                var order = Order.Create(
                    orderId,
                    customerId,
                    orderName,
                    shippingAddress,
                    shippingAddress, // Same as billing address
                    payment
                );

                // Add realistic toy combinations to orders
                var itemCount = random.Next(1, 4); // 1-3 items per order
                var selectedProducts = sampleProducts.OrderBy(x => random.Next()).Take(itemCount);

                foreach (var product in selectedProducts)
                {
                    // Realistic quantities for toys (1-2 usually)
                    var quantity = random.Next(1, 3);
                    var productId = ProductId.Of(product.Id);

                    order.Add(productId, quantity, product.Price);
                }

                // More realistic order status distribution
                var statuses = new[] {
                    OrderStatus.Completed, OrderStatus.Completed, OrderStatus.Completed, // 60% completed
                    OrderStatus.Pending, OrderStatus.Pending, // 40% pending
                    OrderStatus.Cancelled // 20% cancelled
                };
                var randomStatus = statuses[random.Next(statuses.Length)];

                // Use reflection to set the status (since it might be private set)
                var statusProperty = typeof(Order).GetProperty(nameof(Order.Status));
                statusProperty?.SetValue(order, randomStatus);

                // Set creation date in the past (last 60 days for order history)
                var createdDaysAgo = random.Next(0, 60);
                var createdAt = DateTime.UtcNow.AddDays(-createdDaysAgo);

                // Use reflection to set created date
                var createdAtProperty = typeof(Order).GetProperty("CreatedAt") ?? typeof(Order).GetProperty("Created");
                if (createdAtProperty != null && createdAtProperty.CanWrite)
                {
                    createdAtProperty.SetValue(order, createdAt);
                }

                orders.Add(order);
                orderCounter++;
            }
        }

        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        logger.LogInformation("✅ Created {OrderCount} sample orders for {CustomerCount} customers",
            orders.Count, customers.Length);
    }
}
