using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Untz.Database;
using Untz.Endpoints;
using Untz.Utility;
using UntzApi.Services.Implementations;
using UntzApi.Services.Interfaces;
using UntzCommon.Database.Models;
using UntzCommon.Mapping;

var builder = WebApplication.CreateBuilder();

//Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UntzDbContext>(_ => _.UseMySQL(builder.Configuration.GetConnectionString("UntzDb")!));
builder.Services.AddIdentity<UntzUser, IdentityRole>().AddEntityFrameworkStores<UntzDbContext>().AddDefaultTokenProviders(); ;

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfService, PdfService>();

builder.Services.ConfigureApplicationCookie(_ =>
{
    _.Events.OnRedirectToLogin = (_) =>
    {
        _.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.FromResult<object?>(default);
    };
});

builder.Services.AddAuthorization(Helper.AddPolicies);

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddLogging();

builder.Services.AddSpaStaticFiles(_ =>
{
    _.RootPath = "dist";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UntzDbContext>();
    if (db.Database.EnsureCreated())
    {
        db.Database.Migrate();
    }
}

await app.AddDefaultIdentityDataAsync();
await app.AddDefaultSystemDataAsync();

var apiGroup = app.MapGroup("/api");
var apiAdminGroup = app.MapGroup("/api/admin").RequireAuthorization("Admin");

//Events
apiGroup.MapGet("/events/mainEvent", EventsHandler.GetMainEventAsync);
apiGroup.MapGet("/events", EventsHandler.GetAllEventsAsync);
apiGroup.MapPost("/events/subscribe", EventsHandler.SubscribeForEventAsync);
apiGroup.MapGet("/events/detailed", EventsHandler.GetAllEventsDetailedAsync);
apiGroup.MapGet("/events/{eventId}/detailed", EventsHandler.GetDetailedEventAsync);
apiAdminGroup.MapPost("/events", EventsHandler.AddEventAsync);
apiAdminGroup.MapPut("/events", EventsHandler.UpadateEventAsync);
apiAdminGroup.MapDelete("/events/{id}", EventsHandler.DeleteEventAsync);

//Images
apiGroup.MapGet("/imagesForEvents/{eventId}", ImagesHandler.GetImagesForEventAsync);

//Login
apiGroup.MapPost("/login", AuthHandler.LoginAsync);
apiGroup.MapPost("/register", AuthHandler.RegisterAsync);
apiGroup.MapGet("/confirmemail/{userId}/{token}", AuthHandler.ConfirmEmail);
apiGroup.MapGet("/logout", AuthHandler.LogoutAsync).RequireAuthorization();
apiAdminGroup.MapGet("/roles", AuthHandler.GetAllRolesAsync).RequireAuthorization();

//User
apiGroup.MapGet("/untzUsers/current", UsersHandler.GetCurrentUntzUserAsync);
apiAdminGroup.MapGet("/untzUsers", UsersHandler.GetAllUntzUsersAsync);
apiGroup.MapGet("/untzUsers/{userId}", UsersHandler.GetUntzUserAsync);
apiAdminGroup.MapGet("/guestUsers", UsersHandler.GetAllGuestUsersAsync);
apiGroup.MapGet("/guestUsers/{userId}", UsersHandler.GetGuestUserAsync);
apiGroup.MapPost("/guestUsers", UsersHandler.CreateGuestUserAsync);
apiGroup.MapPut("/untzUsers", UsersHandler.UpdateUntzUserAsync);
apiAdminGroup.MapDelete("/untzUsers/{userId}/delete", UsersHandler.DeleteUntzUserAsync);
apiAdminGroup.MapDelete("/guestUsers/{userId}/delete", UsersHandler.DeleteGuestUserAsync);

//Tickets
apiGroup.MapGet("/tickets/{eventId}", TicketsHandler.GetTicketsForEventAsync);

//TicketsPurchase
apiGroup.MapGet("/purchase/{purchasedReferenceId}/ticket/{ticketReferenceId}", TicketsPurchaseHandler.GetPurchasedTicketAloneByReference);
apiGroup.MapGet("/purchase/{referenceId}", TicketsPurchaseHandler.GetPurchasedTicketByReference);
apiGroup.MapGet("/purchase/{referenceId}/detailed", TicketsPurchaseHandler.GetPurchasedTicketByReferenceDetailed);
apiGroup.MapPost("/purchase", TicketsPurchaseHandler.PurchaseAsync);
apiGroup.MapGet("/purchase", TicketsPurchaseHandler.GetPurchasedTicketsAsync);
apiAdminGroup.MapGet("/purchase/user/{id}", TicketsPurchaseHandler.GetPurchasedTicketsForUserAsync);
apiAdminGroup.MapGet("/purchase/guestuser/{id}", TicketsPurchaseHandler.GetPurchasedTicketsForGuestAsync);
apiAdminGroup.MapDelete("/purchase/{id}/delete", TicketsPurchaseHandler.DeleteTicketPurchaseAsync);
apiGroup.MapGet("purchase/payment_return", TicketsPurchaseHandler.HandlePaymentReturnAsync);
apiGroup.MapGet("purchase/payment_cancel", TicketsPurchaseHandler.HandlePaymentCancelAsync);
apiGroup.MapPost("purchase/payment_notify", TicketsPurchaseHandler.HandlePaymentNotifyAsync);
apiGroup.MapGet("purchase/{referenceId}/admit", TicketsPurchaseHandler.AdmitAsync).RequireAuthorization("Admin/Sales");

//PaymentMethods
apiGroup.MapGet("/paymentMethods", PaymentMethodsHandler.GetPaymentMethodsAsync);

if (true/*app.Environment.IsDevelopment()*/)
{
    apiGroup.MapGet("/dev/login", Helper.LoginAsync);
    apiGroup.MapGet("/dev/logout", Helper.LogoutAsync);
    apiGroup.MapGet("/dev/getClaims", Helper.GetClaims).RequireAuthorization();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
#pragma warning disable ASP0014
app.UseEndpoints(_ => { });
app.MapControllers();
#pragma warning restore ASP0014

app.UseSpaStaticFiles();

app.Use((ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/api"))
    {
        ctx.Response.StatusCode = 404;
        return Task.CompletedTask;
    }
    return next();
});


app.UseSpa(_ =>
{
    if (app.Environment.IsDevelopment())
    {
        _.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    }
});

app.Run();