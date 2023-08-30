using System.ComponentModel.DataAnnotations.Schema;
using Untz.Database.Models;

namespace UntzApi.Database.Models
{
    public class PaymentStatus
    {
        public long Id { get; set; }
        public long MerchantId { get; set; }
        public long OrderId { get; set; }
        public string? PaymentId { get; set; }
        public decimal PayhereAmount { get; set; }
        public string? PayhereCurrency { get; set; }
        public int StatusCode { get; set; }
        public string? Md5Sig { get; set; }
        public string? Method { get; set; }
        public string? StatusMessage { get; set; }
        [ForeignKey("TicketPurchaseId")]
        public TicketPurchase TicketPurchase { get; set; } = default!;
        public long TicketPurchaseId { get; set; }

    }
}
