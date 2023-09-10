namespace UntzCommon.Models.Dtos;

public record EventDto(long Id, string Name, string? Description, DateTime CreatedDate, DateTime? PreSaleStartDate, string Location, DateTime EventStartTime, string? Entrance, bool IsActive, MainEventDto? MainEvent, IEnumerable<TicketDto> Tickets, IEnumerable<ImageDto>? Images);
public class ImageDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Base64Content { get; set; }
    public string? FilePath { get; set; }
}
public record MainEventDto(long Id, bool IsActive);
public record TicketDto(long Id, string Name, decimal Price);
public record UntzUserLoginDto(string Username, string Password);
public record GuestUserDto(long Id, string FirstName, string LastName, string PhoneNumber, string Email);
public record TicketPurchaseDto(long Id, long TicketId, int NoOfTickets, long PaymentMethodId, long GuestUserId);
public record PaymentMethodDto(long Id, string Name, string? Description, bool IsActive);
public record PaymentGatewayBodyDto(string merchant_id, string return_url, string cancel_url, string notify_url, string first_name, string last_name, string email, string phone, string address, string city, string country, string order_id, string items, string currency, string amount, string hash, bool custom_1, long custom_2);
public record EventSubscriptionDto(long Id, string Email, string PhoneNumber, long EventId);
public class UntzUserDto
{
    public string Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? Password { get; set; }
    public string? Role { get; set; }
    public bool IsByAdmin { get; set; }
}
public class UntzCurrentLoggedInUserDto
{
    public string Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? Password { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
public class PaymentAcknowledgement
{
    public string merchant_id { get; set; } = default!;
    public long order_id { get; set; } = default!;
    public string payment_id { get; set; } = default!;
    public decimal payhere_amount { get; set; } = default!;
    public string payhere_currency { get; set; } = default!;
    public int status_code { get; set; } = default!;
    public string md5sig { get; set; } = default!;
    public string method { get; set; } = default!;
    public string status_message { get; set; } = default!;
    public bool custom_1 { get; set; }
    public long custom_2 { get; set; } = default!;
}