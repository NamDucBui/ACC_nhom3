namespace Travel.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public string? DetailUrl { get; set; }
        public decimal? Price { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
