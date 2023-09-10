namespace UntzEmailPdfEngine.Models
{
    public class SendPdfDto
    {
        public long TicketPurchasedId { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
