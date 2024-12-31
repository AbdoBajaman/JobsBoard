using JobsBoard.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;


namespace JobsBoard.DataBaseSeeder
{
    public class DataSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<JobsBoardUser> _userManager;

        public DataSeeder(RoleManager<IdentityRole> roleManager, UserManager<JobsBoardUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            // Define default roles
            var roles = new List<string> { "مدير", "باحث عن عمل", "صاحب عمل" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public async Task SeedAdminUserAsync()
        {
            // Define admin user details
            var adminEmail = "abdo99669@gmail.com";
            var adminPassword = "123456789";
            var adminRole = "مدير";

            // Ensure the مدير role exists
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Check if the user already exists
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new JobsBoardUser
                {
                    FirstName = "Abdulrahman",
                    LastName = "Bajaman",
                    NormalizedEmail = adminEmail.ToUpper(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    UserType = adminRole
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors));
                }
            }

            // Assign the مدير role to the admin user
            if (!await _userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await _userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        public async Task SeedDataAsync()
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }
    }
}
