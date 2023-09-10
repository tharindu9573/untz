using System.ComponentModel.DataAnnotations;

namespace UntzCommon.Database.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }
        public string Name { get; set; } = default!;
        public Event Event { get; set; } = default!;
    }
}
