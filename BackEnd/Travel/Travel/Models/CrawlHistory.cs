using System.ComponentModel.DataAnnotations;

namespace Travel.Models
{
    public class CrawlHistory
    {
        public int Id { get; set; }
        
        // Liên kết với CrawlConfig
        public int CrawlConfigId { get; set; }
        public CrawlConfig CrawlConfig { get; set; } = null!;
        
        // Thông tin crawl
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public int ToursFound { get; set; } = 0;           // Số tour tìm thấy
        public int ToursSaved { get; set; } = 0;           // Số tour lưu thành công
        public int ToursSkipped { get; set; } = 0;         // Số tour bỏ qua (trùng lặp)
        
        // Trạng thái
        public string Status { get; set; } = "Running";    // Running, Completed, Failed
        public string? ErrorMessage { get; set; }          // Lỗi nếu có
        
        // Thông tin bổ sung
        public string? SourceUrl { get; set; }             // URL được crawl
        public int PagesCrawled { get; set; } = 0;         // Số trang đã crawl
        public TimeSpan? Duration { get; set; }            // Thời gian crawl
    }
} 