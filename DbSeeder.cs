using Microsoft.AspNetCore.Identity;
using StarEvents.Models;

namespace StarEvents.Data
{
    public static class DbSeeder
    {
        public static async Task SeedData(ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Create Roles
            string[] roleNames = { "Admin", "Organizer", "Customer" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create Admin User
            var adminUser = await userManager.FindByEmailAsync("admin@starevents.com");
            if (adminUser == null)
            {
                var admin = new User
                {
                    UserName = "admin@starevents.com",
                    Email = "admin@starevents.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = "Admin",
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(admin, "Admin123!");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed Event Categories
            if (!context.EventCategories.Any())
            {
                var categories = new List<EventCategory>
                {
                    new EventCategory { CategoryName = "Music Concert", Description = "Live music performances and concerts", IsActive = true },
                    new EventCategory { CategoryName = "Theatre Show", Description = "Drama, comedy and theatrical performances", IsActive = true },
                    new EventCategory { CategoryName = "Cultural Event", Description = "Traditional and cultural celebrations", IsActive = true },
                    new EventCategory { CategoryName = "Sports Event", Description = "Sports competitions and tournaments", IsActive = true },
                    new EventCategory { CategoryName = "Conference", Description = "Business and educational conferences", IsActive = true },
                    new EventCategory { CategoryName = "Workshop", Description = "Educational and skill development workshops", IsActive = true },
                    new EventCategory { CategoryName = "Exhibition", Description = "Art and product exhibitions", IsActive = true }
                };

                context.EventCategories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Sample Venue
            if (!context.Venues.Any())
            {
                var venue = new Venue
                {
                    VenueName = "Colombo Convention Center",
                    Address = "123 Galle Road, Colombo 03",
                    City = "Colombo",
                    Capacity = 5000,
                    ContactPhone = "+94112345678",
                    ContactEmail = "info@ccc.lk",
                    Facilities = "Parking, AC, Restrooms, WiFi, Catering",
                    IsActive = true,
                    CreatedBy = (await userManager.FindByEmailAsync("admin@starevents.com"))?.Id
                };

                context.Venues.Add(venue);
                await context.SaveChangesAsync();
            }
        }
    }
}