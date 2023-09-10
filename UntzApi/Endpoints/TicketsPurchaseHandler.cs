using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Untz.Database;
using UntzApi.Services.Interfaces;
using UntzCommon.Database.Models;
using UntzCommon.Models.Dtos;

namespace Untz.Endpoints
{
    public class TicketsPurchaseHandler
    {
        public static async Task<IResult> PurchaseAsync(TicketPurchaseDto ticketPurchaseDto, ClaimsPrincipal? claimsPrincipal, UserManager<UntzUser> userManager, UntzDbContext dbContext, IPdfService pdfService, IConfiguration configuration)
        {
            TicketPurchase ticketPurchase = new();
            bool isUserSignedIn = false;

            if (claimsPrincipal?.Identity is not null && claimsPrincipal.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(claimsPrincipal);
                ticketPurchase.User = user;
                isUserSignedIn = true;
            }
            else
            {
                ticketPurchase.GuestUser = await dbContext.GuestUsers.FirstOrDefaultAsync(_ => _.Id.Equals(ticketPurchaseDto.GuestUserId));
            }

            var uniqueReference = pdfService.GenerateUniqueReference();
            ticketPurchase.Reference = uniqueReference;
            ticketPurchase.CreatedTime = DateTime.Now;
            ticketPurchase.Ticket = (await dbContext.Tickets.FirstOrDefaultAsync(_ => _.Id.Equals(ticketPurchaseDto.TicketId)))!;
            ticketPurchase.PaymentMethod = (await dbContext.PaymentMethods.FirstOrDefaultAsync(_ => _.Id.Equals(ticketPurchaseDto.PaymentMethodId)))!;
            ticketPurchase.NoOfTickets = ticketPurchaseDto.NoOfTickets;
            ticketPurchase.Sum = ticketPurchase.Ticket.Price * ticketPurchase.NoOfTickets;

            var entity = await dbContext.TicketPurchases.AddAsync(ticketPurchase);
            await dbContext.SaveChangesAsync();

            //Generate body to be sent to the payment gateway
            PaymentGatewayBodyDto paymentGatewayBodyDto = new(configuration["merchant_id"]!,
                configuration["payment_return_url"]!,
                configuration["payment_cancel_url"]!,
                configuration["payment_notify_url"]!,
                isUserSignedIn ? ticketPurchase.User!.FirstName : ticketPurchase.GuestUser!.FirstName,
                isUserSignedIn ? ticketPurchase.User!.LastName : ticketPurchase.GuestUser!.LastName,
                (isUserSignedIn ? ticketPurchase.User!.Email : ticketPurchase.GuestUser!.Email)!,
                (isUserSignedIn ? ticketPurchase.User!.PhoneNumber : ticketPurchase.GuestUser!.PhoneNumber)!,
                "DefaultValue",
                "DefaultValue",
                "DefaultValue",
                uniqueReference,
                "DefaultValue",
                "LKR",
                ticketPurchase.Sum.ToString(),
                pdfService.GenerateHash(uniqueReference, ticketPurchase.Sum, "LKR"),
                isUserSignedIn,
                entity.Entity.Id
                );

            return Results.Ok(paymentGatewayBodyDto);
        }

        public static IResult HandlePaymentReturnAsync(string order_id, IConfiguration configuration)
        {
            return Results.Redirect($"{configuration["host_name"]}/ticketpurchased/{order_id}");
        }

        public static IResult HandlePaymentCancelAsync([FromServices] ILogger<TicketsPurchaseHandler> logger)
        {
            logger.Log(LogLevel.Information, "payment cancel route works");
            return Results.Ok("This is the payment cancel route");
        }

        public static IResult HandlePaymentNotifyAsync(object? returnObj, [FromServices] ILogger<TicketsPurchaseHandler> logger)
        {
            logger.Log(LogLevel.Information, JsonSerializer.Serialize(returnObj));
            return Results.Ok(returnObj);
        }

        public static async Task<IResult> GetPurchasedTicketsAsync(ClaimsPrincipal? claimsPrincipal, UntzDbContext dbContext)
        {
            if (claimsPrincipal?.Identity is not null && claimsPrincipal.Identity.IsAuthenticated)
            {
                var ticketPurchases = await dbContext.TicketPurchases.Where(_ => _.User != null && _.User.UserName!.Equals(claimsPrincipal.Identity.Name))
                    .Include(_ => _.PaymentMethod)
                    .Include(_ => _.User)
                    .Include(_ => _.GuestUser)
                    .Include(_ => _.Ticket)
                    .ThenInclude(_ => _.Event)
                    .ToListAsync();

                var ticketPurchasesJson = JsonSerializer.Serialize(ticketPurchases, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                return Results.Ok(ticketPurchasesJson);
            }

            return Results.Unauthorized();
        }

        public static async Task<IResult> GetPurchasedTicketsForUserAsync(string id, UntzDbContext dbContext)
        {

            var ticketPurchases = await dbContext.TicketPurchases.Where(_ => _.User != null && _.User.Id!.Equals(id))
                .Include(_ => _.Recipt)
                .Include(_ => _.QrCode)
                .Include(_ => _.PaymentMethod)
                .Include(_ => _.User)
                .Include(_ => _.Ticket)
                .ThenInclude(_ => _.Event)
                .ToListAsync();

            var ticketPurchasesJson = JsonSerializer.Serialize(ticketPurchases, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return Results.Ok(ticketPurchasesJson);
        }

        public static async Task<IResult> GetPurchasedTicketsForGuestAsync(int id, UntzDbContext dbContext)
        {
            var ticketPurchases = await dbContext.TicketPurchases.Where(_ => _.GuestUser != null && _.GuestUser.Id!.Equals(id))
                .Include(_ => _.Recipt)
                .Include(_ => _.QrCode)
                .Include(_ => _.PaymentMethod)
                .Include(_ => _.GuestUser)
                .Include(_ => _.Ticket)
                .ThenInclude(_ => _.Event)
                .ToListAsync();

            var ticketPurchasesJson = JsonSerializer.Serialize(ticketPurchases, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return Results.Ok(ticketPurchasesJson);
        }

        public static async Task<IResult> GetPurchasedTicketByReference(string referenceId, UntzDbContext dbContext)
        {
            var ticketPurchases = await dbContext.TicketPurchases.Where(_ => _.Reference.Equals(referenceId))
                .Include(_ => _.PaymentMethod)
                .Include(_ => _.User)
                .Include(_ => _.GuestUser)
                .Include(_ => _.Ticket)
                .ThenInclude(_ => _.Event)
                .FirstOrDefaultAsync();

            var ticketPurchasesJson = JsonSerializer.Serialize(ticketPurchases, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return Results.Ok(ticketPurchasesJson);
        }

        public static async Task<IResult> GetPurchasedTicketByReferenceDetailed(string referenceId, UntzDbContext dbContext)
        {
            var ticketPurchases = await dbContext.TicketPurchases.Where(_ => _.Reference.Equals(referenceId))
                .Include(_ => _.Recipt)
                .Include(_ => _.QrCode)
                .Include(_ => _.PaymentMethod)
                .Include(_ => _.User)
                .Include(_ => _.GuestUser)
                .Include(_ => _.Ticket)
                .ThenInclude(_ => _.Event)
                .FirstOrDefaultAsync();

            var ticketPurchasesJson = JsonSerializer.Serialize(ticketPurchases, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return Results.Ok(ticketPurchasesJson);
        }

        public static async Task<IResult> GetPurchasedTicketAloneByReference(string purchasedReferenceId, string ticketReferenceId, UntzDbContext dbContext)
        {
            var ticketPurchases = await dbContext.TicketPurchases.Where(_ => _.Reference.Equals(purchasedReferenceId))
                .Include(_ => _.QrCode!.Where(_ => _.Reference.Equals(ticketReferenceId)))
                .Include(_ => _.User)
                .Include(_ => _.GuestUser)
                .Include(_ => _.Ticket)
                .ThenInclude(_ => _.Event)
                .FirstOrDefaultAsync();

            var ticketPurchasesJson = JsonSerializer.Serialize(ticketPurchases, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return Results.Ok(ticketPurchasesJson);
        }

        public static async Task<IResult> DeleteTicketPurchaseAsync(long id, UntzDbContext dbContext)
        {
            var purchased = await dbContext.TicketPurchases.FirstOrDefaultAsync(_ => _.Id.Equals(id));
            if (purchased is null)
                return Results.BadRequest();

            dbContext.TicketPurchases.Remove(purchased);

            await dbContext.SaveChangesAsync();

            return Results.Ok(true);
        }

        public static async Task<IResult> AdmitAsync(long referenceId, UntzDbContext dbContext)
        {
            var ticket = await dbContext.QrCodes.FirstOrDefaultAsync(_ => _.Reference.Equals(referenceId.ToString()));

            if (ticket is null)
                return Results.BadRequest();

            ticket.IsAdmitted = true;
            await dbContext.SaveChangesAsync();
            return Results.Ok(true);
        }
    }
}
