using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using HotelAurelia.Areas.Identity.Data;

public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        const string GerantRoleName = "GERANT";
        const string GerantUserName = "Gerant";
        const string GerantEmail = "Gerant@gmail.com";
        const string GerantPassword = "Gerant@4lid";

        // Ensure role exists
        if (!await roleManager.RoleExistsAsync(GerantRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(GerantRoleName));
        }

        // Ensure user exists
        var Gerant = await userManager.FindByNameAsync(GerantUserName);
        if (Gerant == null)
        {
            Gerant = new User
            {
                UserName = GerantUserName,
                Email = GerantEmail,
                Nom = "Gerant",
                Prenom = "Gerant",
                Tel = "0612345678",
                Role = "Gerant",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(Gerant, GerantPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(Gerant, GerantRoleName);
            }
            else
            {
                // log errors from result.Errors
            }
        }
        else
        {
            // Ensure user is in role
            if (!await userManager.IsInRoleAsync(Gerant, GerantRoleName))
            {
                await userManager.AddToRoleAsync(Gerant , GerantRoleName);
            }
        }
    }
}
