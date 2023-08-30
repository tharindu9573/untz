using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Untz.Database;
using Untz.Database.Models;
using Untz.Endpoints.Dtos;

namespace Untz.Endpoints
{
    public class UsersHandler
    {
        public static async Task<IResult> GetCurrentUntzUserAsync(ClaimsPrincipal? claimsPrincipal, SignInManager<UntzUser> signInManager, IMapper mapper)
        {
            if (claimsPrincipal is not null && signInManager.IsSignedIn(claimsPrincipal))
            {
                var user = await signInManager.UserManager.GetUserAsync(claimsPrincipal);
                var roles = claimsPrincipal.Claims.Where(_ => _.Type.Equals(ClaimTypes.Role)).Select(_ => _.Value);

                var currentUser = mapper.Map<UntzCurrentLoggedInUserDto>(user);
                currentUser.Roles = roles;

                return Results.Ok(currentUser);
            }

            return Results.NoContent();
        }

        public static async Task<IResult> GetAllUntzUsersAsync(UserManager<UntzUser> userManager, IMapper mapper)
        {
            var users = await userManager.Users.Where(_ => _.EmailConfirmed).ToListAsync();
            return Results.Ok(mapper.Map<List<UntzUserDto>>(users));
        }

        public static async Task<IResult> GetUntzUserAsync(string userId, UserManager<UntzUser> userManager, IMapper mapper)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(_ => _.Id.Equals(userId));

            if (user is not null)
                return Results.Ok(mapper.Map<UntzUserDto>(user));

            return Results.NotFound("User not found");
        }

        public static async Task<IResult> GetAllGuestUsersAsync(UntzDbContext dbContext, IMapper mapper)
        {
            var users = await dbContext.GuestUsers.ToListAsync();
            return Results.Ok(mapper.Map<List<GuestUserDto>>(users));
        }

        public static async Task<IResult> GetGuestUserAsync(long userId, UntzDbContext dbContext, IMapper mapper)
        {
            var user = await dbContext.GuestUsers.FirstOrDefaultAsync(_ => _.Id.Equals(userId));

            if (user is not null)
                return Results.Ok(mapper.Map<GuestUserDto>(user));

            return Results.NotFound("User not found");
        }

        public static async Task<IResult> CreateGuestUserAsync(GuestUserDto guestUserDto, UntzDbContext dbContext, IMapper mapper)
        {
            var user = mapper.Map<GuestUser>(guestUserDto);

            var createdUser = await dbContext.GuestUsers.AddAsync(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(createdUser.Entity);
        }

        public static async Task<IResult> DeleteGuestUserAsync(long userId, UntzDbContext dbContext, IMapper mapper)
        {
            var user = await dbContext.GuestUsers.FirstOrDefaultAsync(_ => _.Id.Equals(userId));

            if (user is null)
                return Results.NotFound("User not found");

            dbContext.GuestUsers.Remove(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(true);
        }

        public static async Task<IResult> UpdateUntzUserAsync(UntzUserDto untzUserDto, UserManager<UntzUser> userManager, IMapper mapper)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(_ => _.Id.Equals(untzUserDto.Id));

            if (user is null)
                return Results.NotFound("User not found");

            user.FirstName = untzUserDto.FirstName;
            user.LastName = untzUserDto.LastName;
            user.PhoneNumber = untzUserDto.PhoneNumber;
            user.Email = untzUserDto.Email;

            var result = await userManager.UpdateAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var currentUser = mapper.Map<UntzCurrentLoggedInUserDto>(user);
            currentUser.Roles = roles;

            if (!result.Succeeded)
                return Results.Problem();

            return Results.Ok(currentUser);
        }

        public static async Task<IResult> DeleteUntzUserAsync(string userId, UserManager<UntzUser> userManager, IMapper mapper)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(_ => _.Id.Equals(userId));

            if (user is null)
                return Results.NotFound("User not found");

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return Results.Problem();

            return Results.Ok(true);
        }
    }
}
