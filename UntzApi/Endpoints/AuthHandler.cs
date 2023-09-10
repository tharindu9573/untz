using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Untz.Database;
using UntzApi.Services.Interfaces;
using UntzCommon.Database.Models;
using UntzCommon.Models.Dtos;

namespace Untz.Endpoints
{
    public class AuthHandler
    {
        public static async Task<IResult> LoginAsync(UntzUserLoginDto untzUserLoginDto, SignInManager<UntzUser> signInManager, UserManager<UntzUser> userManager)
        {
            var user = await userManager.FindByNameAsync(untzUserLoginDto.Username);
            if (user is not null && await userManager.IsEmailConfirmedAsync(user))
            {
                var signinResult = await signInManager.PasswordSignInAsync(untzUserLoginDto.Username, untzUserLoginDto.Password, true, false);

                if (signinResult.Succeeded)
                    return Results.Ok(true);
            }

            return Results.Unauthorized();
        }

        public static async Task<IResult> RegisterAsync(UntzUserDto untzUserDto, UserManager<UntzUser> userManager, UntzDbContext dbContext, IEmailService emailService, IMapper mapper, IConfiguration configuration)
        {
            var user = mapper.Map<UntzUser>(untzUserDto);
            if (untzUserDto.IsByAdmin)
                user.EmailConfirmed = true;

            var createdResult = await userManager.CreateAsync(user, untzUserDto.Password!);

            if (createdResult.Succeeded)
            {
                var createdUser = userManager.Users.FirstOrDefault(_ => _.UserName!.Equals(untzUserDto.UserName));
                if (untzUserDto.Role is not null)
                {
                    await userManager.AddToRoleAsync(createdUser!, untzUserDto.Role);
                }

                if (!untzUserDto.IsByAdmin)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = Base64UrlEncoder.Encode(token);
                    var emailConfirmationLink = $"{configuration["host_name"]!}/api/confirmemail/{createdUser!.Id}/{encodedToken}";
                    await emailService.SendEmailAsync("Confirm your email", emailConfirmationLink, createdUser.Email!);
                }

                return Results.Created("/", mapper.Map<UntzUserDto>(createdUser));
            }

            return Results.Conflict(createdResult.Errors);
        }

        public static async Task<IResult> ConfirmEmail(string userId, string token, UserManager<UntzUser> userManager, IConfiguration configuration)
        {
            if (userId is null || token is null)
            {
                return Results.BadRequest("Invalid token");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return Results.NotFound("No user found");
            }

            var decodedToken = Base64UrlEncoder.Decode(token);
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Results.Redirect($"{configuration["host_name"]!}/auth/login");
            }

            return Results.Problem("Unable to verify the email");
        }

        public static async Task<IResult> LogoutAsync(ClaimsPrincipal? user, SignInManager<UntzUser> signInManager)
        {
            if (user is not null && signInManager.IsSignedIn(user))
            {
                await signInManager.SignOutAsync();
                return Results.Ok(true);
            }

            return Results.Unauthorized();
        }

        public static async Task<IResult> GetAllRolesAsync(UntzDbContext dbContext)
        {
            return Results.Ok(await dbContext.Roles.Select(_ => new { _.Id, _.Name }).ToListAsync());
        }
    }
}
