using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Data
{
    public class SampleData
    {
        public static async Task SampleAsync(AppDbContext context)
        {
            var hasher = new PasswordHasher<User>();

            if (!await context.Users.AnyAsync())
            {
                var admin = new User
                {
                    FullName = "Akhil",
                    Email = "akhilbalu2477@gmail.com",
                    Role = UserRoles.Admin
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

                var customer = new User
                {
                    FullName = "Balu",
                    Email = "baluakhil4444@gmail.com",
                    Role = UserRoles.Customer
                };
                customer.PasswordHash = hasher.HashPassword(customer, "Customer@123");

                context.Users.AddRange(admin, customer);
                await context.SaveChangesAsync();
            }

            if (!await context.Products.AnyAsync())
            {
                context.Products.AddRange(
                    new Product { ProductName = "Wireless Mouse", Description = "2.4GHz wireless optical mouse", Sku = "ELEC-MOUSE-001", Category = "Electronics", Price = 799.00m, Quantity = 150 },
                    new Product { ProductName = "Mechanical Keyboard", Description = "87-key mechanical keyboard, blue switches", Sku = "ELEC-KEYB-002", Category = "Electronics", Price = 2499.00m, Quantity = 80 },
                    new Product { ProductName = "27-inch Monitor", Description = "1440p IPS monitor, 75Hz", Sku = "ELEC-MON-003", Category = "Electronics", Price = 15999.00m, Quantity = 40 },
                    new Product { ProductName = "USB-C Hub", Description = "7-in-1 USB-C hub with HDMI", Sku = "ELEC-HUB-004", Category = "Electronics", Price = 1299.00m, Quantity = 200 },
                    new Product { ProductName = "Notebook (A5)", Description = "Ruled notebook, 200 pages", Sku = "STAT-NOTE-005", Category = "Stationery", Price = 99.00m, Quantity = 500 },
                    new Product { ProductName = "Gel Pen Set", Description = "Pack of 10 assorted colors", Sku = "STAT-PEN-006", Category = "Stationery", Price = 149.00m, Quantity = 300 },
                    new Product { ProductName = "Office Chair", Description = "Ergonomic mesh-back office chair", Sku = "FURN-CHAIR-007", Category = "Furniture", Price = 8999.00m, Quantity = 25 },
                    new Product { ProductName = "Standing Desk", Description = "Electric height-adjustable desk", Sku = "FURN-DESK-008", Category = "Furniture", Price = 24999.00m, Quantity = 15 },
                    new Product { ProductName = "Water Bottle", Description = "1L stainless steel bottle", Sku = "MISC-BOTL-009", Category = "Lifestyle", Price = 499.00m, Quantity = 350 },
                    new Product { ProductName = "Backpack", Description = "Laptop backpack, 15.6-inch, water resistant", Sku = "MISC-BAG-010", Category = "Lifestyle", Price = 1899.00m, Quantity = 5 }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
