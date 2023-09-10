using System.ComponentModel.DataAnnotations.Schema;

namespace UntzCommon.Database.Models
{
    public class Recipt
    {
        public long Id { get; set; }
        public string ReciptPdf { get; set; } = default!;
        [ForeignKey("TicketPurchaseId")]
        public TicketPurchase TicketPurchase { get; set; } = default!;
        public long TicketPurchaseId { get; set; }
    }
}
