using Microsoft.AspNetCore.Identity;
using SafeLinkAI.Models;

namespace SafeLinkAI.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed roles
            string[] roles = { "Administrador", "Ciudadano" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin user
            const string adminEmail = "admin@safelinkai.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrador SafeLink",
                    Role = "Administrador",
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@1234!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Administrador");
            }

            // Seed demo citizen
            const string citizenEmail = "ciudadano@safelinkai.com";
            if (await userManager.FindByEmailAsync(citizenEmail) == null)
            {
                var citizen = new ApplicationUser
                {
                    UserName = citizenEmail,
                    Email = citizenEmail,
                    FullName = "Juan Pérez",
                    Role = "Ciudadano",
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await userManager.CreateAsync(citizen, "Citizen@1234!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(citizen, "Ciudadano");
            }
        }
    }
}
