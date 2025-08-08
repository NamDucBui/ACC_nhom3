namespace Travel.Models
{
    public class CrawlConfig
    {
        public int Id { get; set; }
        
        // Thông tin nguồn
        public string SourceName { get; set; } = "";                    // Tên nguồn (VD: VNExpress, Tuổi trẻ)
        public string SourceBaseUrl { get; set; } = "";                 // URL cụ thể để crawl
        
        // Selectors cho crawl - Map với các trường trong bảng Tours
        public string SourceContainer { get; set; } = "";               // Container chính chứa danh sách items
        public string SourceEachItemContainer { get; set; } = "";        // Selector cho từng item
        
        // Selectors cho các trường cơ bản
        public string SourceLinkQueryParams { get; set; } = "";          // Selector cho SlugUrl (phải là thẻ <a> để lấy href)
        public string SourceAvatarQueryParams { get; set; } = "";        // Selector cho ImageUrl (phải là thẻ <img> để lấy src)
        public string SourceTitleQueryParams { get; set; } = "";         // Selector cho Name (tiêu đề tour)
        public string SourceSapoQueryParams { get; set; } = "";          // Selector cho Route (lộ trình tour)
        
        // Selectors cho các trường bổ sung của Tour
        public string SourceDurationQueryParams { get; set; } = "";      // Selector cho Duration (thời gian)
        public string SourcePriceOriginalQueryParams { get; set; } = ""; // Selector cho PriceOriginal (giá gốc)
        public string SourcePricePromotionQueryParams { get; set; } = ""; // Selector cho PricePromotion (giá KM)
        public string SourceRatingScoreQueryParams { get; set; } = "";   // Selector cho RatingScore (điểm đánh giá)
        public string SourceRatingCountQueryParams { get; set; } = "";   // Selector cho RatingCount (số đánh giá)
        
        // Cấu hình bổ sung
        public string BackendApi { get; set; } = "";                     // API endpoint để gửi dữ liệu
        public bool HasLoadMore { get; set; } = false;                   // Có nút Load More không
        public string LoadMoreButtonSelector { get; set; } = "";         // Selector cho nút Load More
        public int MaxLoadMore { get; set; } = 3;                        // Số lần click Load More tối đa
        
        // Thông tin quản lý
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;                       // Trạng thái hoạt động
    }
} 