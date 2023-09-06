using UntzApi.Database.Models;

namespace Untz.Database.Models
{
    public class TicketPurchase
    {
        public long Id { get; set; }
        public string Reference { get; set; } = default!;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public virtual UntzUser? User { get; set; }
        public virtual GuestUser? GuestUser { get; set; }
        public virtual Ticket Ticket { get; set; } = default!;
        public virtual PaymentMethod PaymentMethod { get; set; } = default!;
        public int NoOfTickets { get; set; }
        public decimal Sum { get; set; }
        public Recipt? Recipt { get; set; }
        public List<QrCode>? QrCode { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public bool IsProcessCompleted { get; set; } = false;
        public bool IsEmailSent { get; set; } = false;
    }
}
