using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YouNGU.Models.Entities;

namespace YouNGU.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Đảm bảo database đã được tạo
            context.Database.Migrate();

            // Tạo các vai trò
            await CreateRolesAsync(roleManager);

            // Tạo người dùng admin
            await CreateAdminUserAsync(userManager);

            // Tạo danh mục
            await CreateCategoriesAsync(context);
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User", "Creator" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@videohub.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task CreateCategoriesAsync(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Giáo dục", Description = "Video về giáo dục và học tập" },
                    new Category { Name = "Giải trí", Description = "Video giải trí, hài hước" },
                    new Category { Name = "Âm nhạc", Description = "Video âm nhạc, ca nhạc" },
                    new Category { Name = "Thể thao", Description = "Video về thể thao" },
                    new Category { Name = "Công nghệ", Description = "Video về công nghệ, khoa học" },
                    new Category { Name = "Du lịch", Description = "Video về du lịch, khám phá" },
                    new Category { Name = "Ẩm thực", Description = "Video về ẩm thực, nấu ăn" },
                    new Category { Name = "Làm đẹp", Description = "Video về làm đẹp, thời trang" },
                    new Category { Name = "Trò chơi", Description = "Video về trò chơi, game" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}
