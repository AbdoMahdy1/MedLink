using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> users, RoleManager<IdentityRole> roles)
        {
            foreach (var role in new[] { "Admin", "Moderator", "Nurse", "Patient" })
                if (!await roles.RoleExistsAsync(role))
                    await roles.CreateAsync(new IdentityRole(role));

            const string adminEmail = "admin@gmail.com";
            if (await users.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await users.CreateAsync(admin, "Admin@123");
                await users.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
