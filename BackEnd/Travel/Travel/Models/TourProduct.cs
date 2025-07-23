using System;

namespace Travel.Models
{
    public class TourProduct
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Duration { get; set; }
        public string? ImageUrl { get; set; }
        public string DetailUrl { get; set; } = string.Empty;
        public string? RawHtml { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
} 