using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Untz.Database;
using UntzCommon.Database.Models;

namespace Untz.Utility
{
    public static class Helper
    {
        public static void AddPolicies(AuthorizationOptions authorizationOptions)
        {
            authorizationOptions.AddPolicy("Admin", _ =>
            {
                _.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin");
            });

            authorizationOptions.AddPolicy("Admin/Sales", _ =>
            {
                _.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "Sales");
            });
        }

        public static async Task AddDefaultIdentityDataAsync(this WebApplication? webApplication)
        {
            var scope = webApplication!.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UntzUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (!await roleManager.Roles.AnyAsync(_ => _.Name!.Equals("Admin")))
            {
                await roleManager.CreateAsync(new()
                {
                    Name = "Admin"
                });
            }

            if (!await roleManager.Roles.AnyAsync(_ => _.Name!.Equals("Sales")))
            {
                await roleManager.CreateAsync(new()
                {
                    Name = "Sales"
                });
            }

            if (!await userManager.Users.AnyAsync(_ => _.UserName!.Equals("Admin")))
            {
                UntzUser adminUser = new()
                {
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = configuration["admin_phone_number"]!,
                    Email = configuration["email"]!,
                    UserName = "Admin",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, configuration["admin_password"]!);

                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (!await userManager.Users.AnyAsync(_ => _.UserName!.Equals("General")))
            {
                UntzUser generalUser = new()
                {
                    FirstName = "General",
                    LastName = "User",
                    PhoneNumber = configuration["admin_phone_number"]!,
                    Email = configuration["email"]!,
                    UserName = "General",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(generalUser, configuration["admin_password"]!);
            }
        }

        public static async Task AddDefaultSystemDataAsync(this WebApplication? webApplication)
        {
            var scope = webApplication!.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UntzDbContext>();

            if (!await dbContext.PaymentMethods.AnyAsync())
            {
                await dbContext.PaymentMethods.AddAsync(new()
                {
                    Name = "Credit/Debit Card",
                    Description = "Pay by credit or debit cards",
                    IsActive = true,
                });

                await dbContext.SaveChangesAsync();
            }

            var configurations = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            Environment.SetEnvironmentVariable("host_name", configurations["host_name"]);
            if (webApplication.Environment.IsDevelopment())
            {
                Environment.SetEnvironmentVariable("image_upload_path", $"{configurations["image_upload_base_path"]}");
            }
            else
            {
                if (!Directory.Exists($"{Directory.GetCurrentDirectory()}/dist/uploaded"))
                {
                    Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/dist/uploaded");
                }

                Environment.SetEnvironmentVariable("image_upload_path", $"{Directory.GetCurrentDirectory()}/dist/uploaded");
            }
        }

        public static async Task<IResult> LoginAsync(SignInManager<UntzUser> signInManager, IConfiguration configuration)
        {
            var signinResult = await signInManager.PasswordSignInAsync("Admin", configuration["admin_password"]!, true, false);

            if (signinResult.Succeeded)
                return Results.Ok();

            return Results.Unauthorized();
        }

        public async static Task<IResult> LogoutAsync(SignInManager<UntzUser> signInManager)
        {
            await signInManager.SignOutAsync();

            return Results.Ok();
        }

        public static IResult GetClaims(ClaimsPrincipal user)
        {
            return Results.Ok(user.Claims.ToDictionary(_ => _.Type, _ => _.Value));
        }
    }
}
