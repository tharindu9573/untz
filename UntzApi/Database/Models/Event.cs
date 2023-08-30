namespace Untz.Database.Models
{
    public class Event
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PreSaleStartDate { get; set; }
        public string Location { get; set; } = default!;
        public DateTime EventStartTime { get; set; }
        public string? Entrance { get; set; }
        public bool IsActive { get; set; }
        public virtual List<Image> Images { get; set; } = new();
        public virtual List<Ticket> Tickets { get; set; } = new();
        public virtual MainEvent? MainEvent { get; set; }
    }
}
