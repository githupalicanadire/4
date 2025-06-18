using Identity.API.Data;
using Identity.API.Models;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace Identity.API.Configuration;

public static class SeedData
{
    public static async Task EnsureSeedData(IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Seeding Identity Server database...");

        try
        {
            // Seed IdentityServer configuration
            await SeedIdentityServerConfiguration(serviceProvider);

            // Seed users
            await SeedUsers(serviceProvider);

            Log.Information("✅ Identity Server database seeding completed");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "❌ Error during database seeding");
            throw;
        }
    }

    private static async Task SeedIdentityServerConfiguration(IServiceProvider serviceProvider)
    {
        var configurationDbContext = serviceProvider.GetRequiredService<ConfigurationDbContext>();

        try
        {
            // Seed Identity Resources
            if (!await configurationDbContext.IdentityResources.AnyAsync())
            {
                Log.Information("🔑 Seeding identity resources...");
                foreach (var resource in Config.IdentityResources)
                {
                    configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            // Seed API Scopes
            if (!await configurationDbContext.ApiScopes.AnyAsync())
            {
                Log.Information("🔒 Seeding API scopes...");
                foreach (var scope in Config.ApiScopes)
                {
                    configurationDbContext.ApiScopes.Add(scope.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            // Seed API Resources
            if (!await configurationDbContext.ApiResources.AnyAsync())
            {
                Log.Information("🌐 Seeding API resources...");
                foreach (var resource in Config.ApiResources)
                {
                    configurationDbContext.ApiResources.Add(resource.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            // Seed Clients
            if (!await configurationDbContext.Clients.AnyAsync())
            {
                Log.Information("👥 Seeding clients...");
                foreach (var client in Config.Clients)
                {
                    configurationDbContext.Clients.Add(client.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            Log.Information("✅ IdentityServer configuration seeded successfully");
        }
        catch (Exception ex)
        {
            Log.Warning("⚠️ Error seeding IdentityServer configuration: {Error}. Will retry on next startup.", ex.Message);
        }
    }

    // Removed complex manual seeding code - using standard Duende IdentityServer seeding

    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Demo user "swn" - keeping compatibility with current frontend
        var swnUser = await userManager.FindByNameAsync("swn");
        if (swnUser == null)
        {
            Log.Information("👤 Creating demo user 'swn'...");
            swnUser = new ApplicationUser
            {
                UserName = "swn",
                Email = "swn@shopping.com",
                FirstName = "Demo",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(swnUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddClaimsAsync(swnUser, new[]
                {
                    new Claim("sub", swnUser.Id),
                    new Claim("name", swnUser.FullName),
                    new Claim("given_name", swnUser.FirstName),
                    new Claim("family_name", swnUser.LastName),
                    new Claim("email", swnUser.Email),
                    new Claim("role", "customer")
                });
                Log.Information("✅ Demo user 'swn' created successfully");
            }
            else
            {
                Log.Error("❌ Failed to create demo user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Admin user
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            Log.Information("👤 Creating admin user...");
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@shopping.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddClaimsAsync(adminUser, new[]
                {
                    new Claim("sub", adminUser.Id),
                    new Claim("name", adminUser.FullName),
                    new Claim("given_name", adminUser.FirstName),
                    new Claim("family_name", adminUser.LastName),
                    new Claim("email", adminUser.Email),
                    new Claim("role", "admin")
                });
                Log.Information("✅ Admin user created successfully");
            }
        }

        // Only keep admin and one demo user - users should register themselves
        Log.Information("✅ Users can now register themselves via the registration system");
    }
}
