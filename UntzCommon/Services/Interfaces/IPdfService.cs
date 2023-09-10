using UntzCommon.Database.Models;

namespace UntzApi.Services.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]?> GenerateReciptAsync(TicketPurchase ticketPurchase, bool isAuthenticated);
        Task<List<byte[]?>> GenerateTicketsAsync(TicketPurchase ticketPurchase, Dictionary<string, string> qrCodeList, bool isAuthenticated);
        Task<string> GenerateQrCodeAsync(string reference);
        string GenerateHash(string order_id, decimal amount, string currency);

        string GenerateUniqueReference();
    }
}
