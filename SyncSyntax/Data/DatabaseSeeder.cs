using Microsoft.AspNetCore.Identity;

namespace SyncSyntax.Data
{
    public class DatabaseSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Seed roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed admin user
            if (await _userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
                await _userManager.CreateAsync(adminUser, "Admin@123");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed regular user
            if (await _userManager.FindByEmailAsync("user@example.com") == null)
            {
                var regularUser = new IdentityUser { UserName = "user@example.com", Email = "user@example.com" };
                await _userManager.CreateAsync(regularUser, "User@123");
                await _userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }
}
