using System.ComponentModel.DataAnnotations.Schema;

namespace Untz.Database.Models
{
    public class MainEvent
    {

        public long Id { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; } = default!;
        public long EventId { get; set; }
    }
}
