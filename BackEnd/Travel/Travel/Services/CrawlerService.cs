using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Travel.Models;
using Travel.Data;

namespace Travel.Services
{
    public class CrawlerService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CrawlerService> _logger;
        private readonly TelegramNotificationService _telegramService;

        public CrawlerService(AppDbContext context, ILogger<CrawlerService> logger, TelegramNotificationService telegramService)
        {
            _context = context;
            _logger = logger;
            _telegramService = telegramService;
        }

        public async Task<CrawlResult> RunCrawlWithConfigAsync(int configId)
        {
            var config = await _context.CrawlConfigs.FindAsync(configId);
            if (config == null)
            {
                return new CrawlResult { Success = false, Message = "Cấu hình không tồn tại" };
            }

            // Tạo crawl history record
            var crawlHistory = new CrawlHistory
            {
                CrawlConfigId = configId,
                StartedAt = DateTime.Now,
                Status = "Running",
                SourceUrl = config.SourceBaseUrl
            };
            _context.CrawlHistories.Add(crawlHistory);
            await _context.SaveChangesAsync();

            // Gửi thông báo bắt đầu crawl
            await _telegramService.SendCrawlStartNotificationAsync(config.SourceName, config.SourceBaseUrl);

            var startTime = DateTime.Now;
            var toursFound = 0;
            var toursSaved = 0;
            var toursSkipped = 0;
            var pagesCrawled = 0;

            try
            {
                using var driver = new ChromeDriver();
                driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

                // Navigate to the URL
                driver.Navigate().GoToUrl(config.SourceBaseUrl);
                await Task.Delay(3000);

                var allTours = new List<Tour>();
                var processedUrls = new HashSet<string>();

                // Crawl first page
                var tours = await ExtractToursFromPage(driver, config);
                toursFound += tours.Count;
                allTours.AddRange(tours);
                pagesCrawled++;

                // Handle load more if enabled
                if (config.HasLoadMore && !string.IsNullOrEmpty(config.LoadMoreButtonSelector))
                {
                    var loadMoreCount = 0;
                    while (loadMoreCount < config.MaxLoadMore)
                    {
                        try
                        {
                            var loadMoreButton = driver.FindElement(By.CssSelector(config.LoadMoreButtonSelector));
                            if (loadMoreButton != null && loadMoreButton.Displayed)
                            {
                                // Try direct click first
                                try
                                {
                                    loadMoreButton.Click();
                                }
                                catch
                                {
                                    // If direct click fails, try JavaScript click
                                    driver.ExecuteScript("arguments[0].click();", loadMoreButton);
                                }

                                await Task.Delay(5000); // Wait longer for content to load

                                // Check if new content appeared
                                var newTours = await ExtractToursFromPage(driver, config);
                                var newCount = newTours.Count - allTours.Count;
                                
                                if (newCount > 0)
                                {
                                    allTours = newTours; // Update with all tours found
                                    toursFound += newCount;
                                    pagesCrawled++;
                                    loadMoreCount++;
                                }
                                else
                                {
                                    break; // No new content, stop loading more
                                }
                            }
                            else
                            {
                                break; // Load more button not found or not visible
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error clicking load more: {ex.Message}");
                            break;
                        }
                    }
                }

                // Save tours to database
                foreach (var tour in allTours)
                {
                    tour.CrawlConfigId = configId;
                    
                    // Check if tour already exists (by URL)
                    var existingTour = await _context.Tours
                        .FirstOrDefaultAsync(t => t.SlugUrl == tour.SlugUrl && t.CrawlConfigId == configId);
                    
                    if (existingTour == null)
                    {
                        _context.Tours.Add(tour);
                        toursSaved++;
                    }
                    else
                    {
                        toursSkipped++;
                    }
                }

                await _context.SaveChangesAsync();

                // Update crawl history
                crawlHistory.CompletedAt = DateTime.Now;
                crawlHistory.Status = "Completed";
                crawlHistory.ToursFound = toursFound;
                crawlHistory.ToursSaved = toursSaved;
                crawlHistory.ToursSkipped = toursSkipped;
                crawlHistory.PagesCrawled = pagesCrawled;
                crawlHistory.Duration = DateTime.Now - startTime;
                await _context.SaveChangesAsync();

                // Gửi thông báo hoàn thành crawl
                await _telegramService.SendCrawlCompleteNotificationAsync(
                    config.SourceName, 
                    true, 
                    $"Crawl thành công! Tìm thấy: {toursFound}, Lưu: {toursSaved}, Bỏ qua: {toursSkipped}, Trang: {pagesCrawled}",
                    toursFound,
                    toursSaved,
                    toursSkipped,
                    pagesCrawled,
                    DateTime.Now - startTime
                );

                return new CrawlResult 
                { 
                    Success = true, 
                    Message = $"Crawl thành công! Tìm thấy: {toursFound}, Lưu: {toursSaved}, Bỏ qua: {toursSkipped}, Trang: {pagesCrawled}" 
                };
            }
            catch (Exception ex)
            {
                // Update crawl history with error
                crawlHistory.CompletedAt = DateTime.Now;
                crawlHistory.Status = "Failed";
                crawlHistory.ErrorMessage = ex.Message;
                crawlHistory.ToursFound = toursFound;
                crawlHistory.ToursSaved = toursSaved;
                crawlHistory.ToursSkipped = toursSkipped;
                crawlHistory.PagesCrawled = pagesCrawled;
                crawlHistory.Duration = DateTime.Now - startTime;
                await _context.SaveChangesAsync();

                // Gửi thông báo lỗi crawl
                await _telegramService.SendCrawlCompleteNotificationAsync(
                    config.SourceName,
                    false,
                    $"Lỗi crawl: {ex.Message}",
                    toursFound,
                    toursSaved,
                    toursSkipped,
                    pagesCrawled,
                    DateTime.Now - startTime
                );

                return new CrawlResult { Success = false, Message = $"Lỗi crawl: {ex.Message}" };
            }
        }

        private Task<List<Tour>> ExtractToursFromPage(IWebDriver driver, CrawlConfig config)
        {
            var tours = new List<Tour>();
            var items = driver.FindElements(By.CssSelector(config.SourceEachItemContainer));

            foreach (var item in items)
            {
                try
                {
                    var tour = new Tour();

                    // Map các trường từ CrawlConfig
                    tour.ImageUrl = GetImageUrl(item, config.SourceAvatarQueryParams);
                    tour.Name = GetElementText(item, config.SourceTitleQueryParams);
                    tour.SlugUrl = GetElementText(item, config.SourceLinkQueryParams, "href");
                    tour.Route = GetElementText(item, config.SourceSapoQueryParams);
                    tour.Duration = GetElementText(item, config.SourceDurationQueryParams);
                    tour.PriceOriginal = GetElementText(item, config.SourcePriceOriginalQueryParams);
                    tour.PricePromotion = GetElementText(item, config.SourcePricePromotionQueryParams);

                    // Xử lý RatingScore
                    var ratingScoreText = GetElementText(item, config.SourceRatingScoreQueryParams);
                    if (double.TryParse(ratingScoreText?.Replace(",", "."), out var score))
                        tour.RatingScore = score;

                    // Xử lý RatingCount
                    var ratingCountText = GetElementText(item, config.SourceRatingCountQueryParams);
                    if (ratingCountText != null)
                    {
                        var digits = new string(ratingCountText.Where(char.IsDigit).ToArray());
                        if (int.TryParse(digits, out var count))
                            tour.RatingCount = count;
                    }

                    // Chỉ lưu tour có ít nhất tên
                    if (!string.IsNullOrWhiteSpace(tour.Name))
                    {
                        tours.Add(tour);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Lỗi crawl item: {ex.Message}");
                }
            }
            return Task.FromResult(tours);
        }

        private string GetElementText(IWebElement item, string selector, string? attribute = null)
        {
            if (string.IsNullOrEmpty(selector))
                return "";

            try
            {
                var element = item.FindElement(By.CssSelector(selector));
                
                if (attribute != null)
                {
                    return element.GetAttribute(attribute)?.Trim() ?? "";
                }
                else
                {
                    return element.Text?.Trim() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

        private string GetImageUrl(IWebElement item, string selector)
        {
            if (string.IsNullOrEmpty(selector))
                return "";

            try
            {
                var element = item.FindElement(By.CssSelector(selector));
                
                // Thử lấy src trước
                var src = element.GetAttribute("src")?.Trim();
                if (!string.IsNullOrEmpty(src))
                    return src;

                // Thử lấy data-src (lazy loading)
                var dataSrc = element.GetAttribute("data-src")?.Trim();
                if (!string.IsNullOrEmpty(dataSrc))
                    return dataSrc;

                // Thử lấy data-original
                var dataOriginal = element.GetAttribute("data-original")?.Trim();
                if (!string.IsNullOrEmpty(dataOriginal))
                    return dataOriginal;

                // Thử lấy data-lazy-src
                var dataLazySrc = element.GetAttribute("data-lazy-src")?.Trim();
                if (!string.IsNullOrEmpty(dataLazySrc))
                    return dataLazySrc;

                return "";
            }
            catch
            {
                return "";
            }
        }
    }

    public class CrawlResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int TourCount { get; set; }
    }
} 