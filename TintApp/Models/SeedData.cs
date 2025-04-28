using Microsoft.AspNetCore.Identity;

namespace TintApp.Models
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            var roleNames = new[] { "Admin", "SuperAdmin" };

            // Create roles if they don't exist
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }

            // Add SuperAdmin if it doesn't exist already
            var superAdmin = await userManager.FindByNameAsync("superadmin@gmail.com");
            if (superAdmin == null)
            {
                superAdmin = new ApplicationUser { UserName = "superadmin@gmail.com", Email = "superadmin@gmail.com" };
                var result = await userManager.CreateAsync(superAdmin, "SuperAdmin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                }
            }

            // Add Admin if it doesn't exist already
            var admin = await userManager.FindByNameAsync("admin@gmail.com");
            if (admin == null)
            {
                admin = new ApplicationUser { UserName = "admin@gmail.com", Email = "admin@gmail.com" };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
