namespace TravelTourCrawler.DTO
{
    public class CrawlRequestDto
    {
        public string Url { get; set; } = null!;
        public string ListContainerClass { get; set; } = null!;
        public string TitleSelector { get; set; } = "div/a";
        public string ImageSelector { get; set; } = "img";
        public string DetailContainerClass { get; set; } = "item-content-detail";
        public string LabelClass { get; set; } = "item-content-p";
        public string PriceClass { get; set; } = "price-new";
    }
}
