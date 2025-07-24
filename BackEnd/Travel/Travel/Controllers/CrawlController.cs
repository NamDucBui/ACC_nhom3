using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Travel.Models;
using System;
using System.Linq;

namespace Travel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CrawlController(AppDbContext context)
        {
            _context = context;
        }

        // Model nhận từ Frontend
        public class CrawlRequest
        {
            public string Url { get; set; }
        }

        [HttpPost("traveloka-hotel")]
        public async Task<IActionResult> CrawlTravelokaHotel([FromBody] CrawlRequest request)
        {
            if (string.IsNullOrEmpty(request.Url))
                return BadRequest("Thiếu URL.");

            var options = new ChromeOptions();
            // Tạm thời tắt headless để test cho dễ nhìn
            // options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");

            var hotels = new List<TourProduct>();

            using (var driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl(request.Url);

                // Cuộn trang nhiều lần để Traveloka load đủ nội dung
                for (int i = 0; i < 5; i++)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 1000);");
                    await Task.Delay(2000);
                }

                // Đợi tối đa 30s cho đến khi hotel xuất hiện
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                try
                {
                    wait.Until(d =>
                        d.FindElements(By.CssSelector("a[href*='hotel/detail']")).Count > 0);
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("⛔ Không tìm thấy khách sạn sau 30 giây.");
                }

                var hotelCards = driver.FindElements(By.CssSelector("a[href*='hotel/detail']"));
                Console.WriteLine("🛎️ Đã tìm thấy " + hotelCards.Count + " khách sạn.");

                foreach (var card in hotelCards)
                {
                    string name = "", price = "", link = "", img = "";

                    try
                    {
                        name = card.FindElement(By.CssSelector("h4.css-4rbku5.css-901oao.css-cens5h")).Text;
                    }
                    catch { }

                    try
                    {
                        price = card.FindElement(By.CssSelector("h4[style*='color: rgb(249, 109, 1)']")).Text;
                    }
                    catch { }

                    try
                    {
                        link = card.GetAttribute("href");
                    }
                    catch { }

                    try
                    {
                        img = card.FindElement(By.CssSelector("img")).GetAttribute("src");
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(name))
                    {
                        var hotel = new TourProduct
                        {
                            Title = name,
                            Price = ParsePrice(price),
                            DetailUrl = link,
                            ImageUrl = img,
                            SourceId = 1,
                            LastUpdatedAt = DateTime.Now
                        };
                        hotels.Add(hotel);
                        _context.TourProducts.Add(hotel);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Ok(hotels);
        }


        [HttpPost("traveloka-hotel-to-hotels")]
        public async Task<IActionResult> CrawlTravelokaHotelToHotels([FromBody] CrawlRequest request)
        {
            if (string.IsNullOrEmpty(request.Url))
                return BadRequest("Thiếu URL.");

            var options = new ChromeOptions();
            // options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");

            var hotels = new List<Hotel>();

            using (var driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl(request.Url);

                for (int i = 0; i < 5; i++)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 1000);");
                    await Task.Delay(2000);
                }

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                try
                {
                    wait.Until(d =>
                        d.FindElements(By.CssSelector("a[href*='hotel/detail']")).Count > 0);
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("⛔ Không tìm thấy khách sạn sau 30 giây.");
                }

                var hotelCards = driver.FindElements(By.CssSelector("a[href*='hotel/detail']"));
                Console.WriteLine("🛎️ Đã tìm thấy " + hotelCards.Count + " khách sạn.");

                foreach (var card in hotelCards)
                {
                    string name = "", price = "", link = "", img = "", location = "";

                    try
                    {
                        name = card.FindElement(By.CssSelector("h4.css-4rbku5.css-901oao.css-cens5h")).Text;
                    }
                    catch { }

                    try
                    {
                        var h4s = card.FindElements(By.CssSelector("h4"));
                        if (h4s.Count > 1)
                            price = h4s.Last().Text;
                    }
                    catch { }

                    try
                    {
                        link = card.GetAttribute("href");
                    }
                    catch { }

                    try
                    {
                        img = card.FindElement(By.CssSelector("img")).GetAttribute("src");
                    }
                    catch { }

                    try
                    {
                        location = card.FindElement(By.CssSelector("div[style*='background-color: rgb(3, 18, 26)'] div.css-901oao.css-bfa6kz")).Text;
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(name))
                    {
                        var hotel = new Hotel
                        {
                            Name = name,
                            Price = ParsePrice(price),
                            DetailUrl = link,
                            ImageUrl = img,
                            Location = location, // Thêm location vào đây
                            LastUpdatedAt = DateTime.Now
                        };
                        hotels.Add(hotel);
                        _context.Hotels.Add(hotel);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Ok(hotels);
        }

        private decimal? ParsePrice(string priceStr)
        {
            if (string.IsNullOrEmpty(priceStr)) return null;
            var digits = new string(priceStr.Where(char.IsDigit).ToArray());
            if (decimal.TryParse(digits, out var price)) return price;
            return null;
        }
    }
}
