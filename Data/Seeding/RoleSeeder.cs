
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Data.Seeding;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            UserRoles.SuperAdmin,
            UserRoles.PlacementOfficer,
            UserRoles.CompanyHR,
            UserRoles.Student
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    public static async Task SeedSuperAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Admin email or password is missing from User Secrets.");
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = "Super Admin",
                EmailConfirmed = true,
                AccountStatus = AccountStatus.Approved
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, UserRoles.SuperAdmin))
        {
            var result = await userManager.AddToRoleAsync(
                user, UserRoles.SuperAdmin);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
