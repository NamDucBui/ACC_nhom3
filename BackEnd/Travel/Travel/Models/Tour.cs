namespace Travel.Models
{
    public class Tour
    {
        public int Id { get; set; }  // Id có thể lấy theo thứ tự crawl hoặc backend tự sinh

        public string ImageUrl { get; set; } = "";               // Đường dẫn ảnh đại diện
        public string Name { get; set; } = "";                   // Tên tour
        public string SlugUrl { get; set; } = "";                // URL chi tiết tour (dạng slug)

        public string Route { get; set; } = "";                  // Lộ trình tour (ví dụ: Hà Nội - Osaka - Tokyo...)
        public string Duration { get; set; } = "";               // Thời gian tour (ví dụ: 6N5Đ)

        public string PriceOriginal { get; set; } = "";          // Giá gốc (ví dụ: 31.900.000đ)
        public string PricePromotion { get; set; } = "";         // Giá khuyến mãi (ví dụ: 30.900.000đ)

        public double? RatingScore { get; set; }           // Điểm đánh giá (ví dụ: 9.0)
        public int? RatingCount { get; set; }              // Số lượng đánh giá (ví dụ: 33)

        // Liên kết với CrawlConfig
        public int? CrawlConfigId { get; set; }            // ID của cấu hình crawl (nullable)
        public CrawlConfig? CrawlConfig { get; set; }      // Navigation property
    }
}
