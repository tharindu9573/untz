namespace UntzCommon.Database.Models
{
    public class Image
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string FilePath { get; set; } = default!;
        public virtual Event Event { get; set; } = default!;
    }
}
