namespace Untz.Database.Models
{
    public class PaymentMethod
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
