using System.ComponentModel.DataAnnotations.Schema;
using Untz.Database.Models;

namespace UntzApi.Database.Models
{
    public class EventSubscription
    {
        public long Id { get; set; }
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        [ForeignKey("EventId")]
        public Event Event { get; set; } = default!;
        public long EventId { get; set; }

    }
}
