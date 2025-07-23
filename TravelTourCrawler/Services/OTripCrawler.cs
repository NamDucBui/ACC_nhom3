using HtmlAgilityPack;
using TravelTourCrawler.Models;

namespace TravelTourCrawler.Services
{
    public class OTripCrawler : ITourCrawler
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OTripCrawler> _logger;

        public OTripCrawler(HttpClient httpClient, ILogger<OTripCrawler> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Cấu hình HttpClient
            _httpClient.BaseAddress = new Uri("https://otrip.vn/");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<Tour>> CrawlToursAsync()
        {
            try
            {
                _logger.LogInformation("Starting to crawl tours from OTrip");
                var tours = new List<Tour>();

                // Crawl 2 trang đầu tiên
                for (int page = 1; page <= 2; page++)
                {
                    var response = await _httpClient.GetAsync($"/tours/tour-noi-dia?page={page}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(content);

                    var tourNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'item-category')]");

                    if (tourNodes != null)
                    {
                        foreach (var tourNode in tourNodes)
                        {
                            var tour = new Tour { Source = "OTrip" };

                            // Title và URL
                            var titleNode = tourNode.SelectSingleNode(".//h2/a");
                            if (titleNode != null)
                            {
                                tour.Title = titleNode.InnerText.Trim();
                                tour.Url = titleNode.GetAttributeValue("href", "");
                                tour.Id = $"otrip-{tour.Url?.GetHashCode()}";
                            }

                            // Image
                            var imageNode = tourNode.SelectSingleNode(".//img");
                            if (imageNode != null)
                            {
                                tour.ImageUrl = imageNode.GetAttributeValue("src", "");
                            }

                            // Details
                            var detailNodes = tourNode.SelectNodes(".//div[contains(@class, 'item-content-detail')]");
                            if (detailNodes != null)
                            {
                                foreach (var detailNode in detailNodes)
                                {
                                    var label = detailNode.SelectSingleNode(".//span[contains(@class, 'item-content-p')]")?.InnerText.Trim();
                                    var value = detailNode.SelectSingleNode(".//span[not(contains(@class, 'item-content-p'))]")?.InnerText.Trim();

                                    if (label != null && value != null)
                                    {
                                        if (label.Contains("Điểm khởi hành:"))
                                            tour.DeparturePoint = value;
                                        else if (label.Contains("Điểm đến:"))
                                            tour.Destination = value;
                                        else if (label.Contains("Lịch trình:"))
                                            tour.Duration = value;
                                        else if (label.Contains("Khởi hành:"))
                                            tour.DepartureTime = value;
                                        else if (label.Contains("Phương tiện:"))
                                            tour.Transportation = value;
                                    }
                                }
                            }

                            // Price
                            var priceNode = tourNode.SelectSingleNode(".//p[contains(@class, 'price-new')]");
                            if (priceNode != null)
                            {
                                tour.Price = priceNode.InnerText.Trim();
                            }

                            tours.Add(tour);
                        }
                    }

                    // Delay giữa các request để tránh bị block
                    await Task.Delay(2000);
                }

                _logger.LogInformation($"Crawled {tours.Count} tours from OTrip");
                return tours;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crawling OTrip");
                return new List<Tour>();
            }
        }
    }
}