using System.ComponentModel.DataAnnotations.Schema;
using Untz.Database.Models;

namespace UntzApi.Database.Models
{
    public class QrCode
    {
        public long Id { get; set; }
        public string Reference { get; set; } = default!;
        public string QrCodeImage { get; set; } = default!;
        [ForeignKey("TicketPurchaseId")]
        public TicketPurchase TicketPurchase { get; set; } = default!;
        public long TicketPurchaseId { get; set; }
    }
}
