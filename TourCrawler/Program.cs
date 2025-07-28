using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

class Config
{
    public string Url { get; set; }
    public string ContainerSelector { get; set; }
    public string ItemSelector { get; set; }
    public Dictionary<string, string> Fields { get; set; }
    public string LoadMoreButtonSelector { get; set; }
    public int MaxLoadMore { get; set; }
    public string BackendApi { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless"); // Ẩn trình duyệt
        using var driver = new ChromeDriver(options);
        driver.Navigate().GoToUrl(config.Url);

        Console.WriteLine("⏳ Loading page...");

        // Click "Load More" nhiều lần
        for (int i = 0; i < config.MaxLoadMore; i++)
        {
            try
            {
                var itemsBefore = driver.FindElements(By.CssSelector(config.ItemSelector)).Count;
                var btn = driver.FindElement(By.CssSelector(config.LoadMoreButtonSelector));
                btn.Click();
                // Chờ cho đến khi số lượng bản ghi tăng lên hoặc hết timeout
                int waited = 0;
                int itemsAfter = itemsBefore;
                while (waited < 10000) // tối đa 10 giây
                {
                    Thread.Sleep(500);
                    waited += 500;
                    itemsAfter = driver.FindElements(By.CssSelector(config.ItemSelector)).Count;
                    if (itemsAfter > itemsBefore)
                        break;
                }
                Console.WriteLine($"Sau lần bấm Load More thứ {i + 1}, số bản ghi hiện có: {itemsAfter}");
            }
            catch
            {
                Console.WriteLine("❌ Không tìm thấy nút Load More nữa.");
                break;
            }
        }

        Thread.Sleep(2000); // Đợi trang load hoàn tất

        var items = driver.FindElements(By.CssSelector(config.ItemSelector));
        Console.WriteLine($"✅ Tổng số bản ghi crawl được: {items.Count}");

        var list = new List<Dictionary<string, string>>();

        foreach (var item in items)
        {
            var record = new Dictionary<string, object>();
            foreach (var field in config.Fields)
            {
                try
                {
                    var element = item.FindElement(By.CssSelector(field.Value));
                    string value = "";
                    if (field.Key == "ImageUrl" && element.TagName == "img")
                    {
                        value = element.GetAttribute("src");
                        if (string.IsNullOrWhiteSpace(value))
                            value = element.GetAttribute("data-src");
                    }
                    else if (field.Key == "SlugUrl" && element.TagName == "a")
                    {
                        value = element.GetAttribute("href");
                    }
                    else
                    {
                        value = element.TagName == "img" ? element.GetAttribute("src") : element.Text;
                    }
                    value = value?.Trim() ?? "";
                    record[field.Key] = value;
                    // Console.WriteLine($"{field.Key}: {value}"); // Bỏ log từng trường
                }
                catch
                {
                    record[field.Key] = "";
                    // Console.WriteLine($"{field.Key}: [NOT FOUND]"); // Bỏ log từng trường
                }
            }
            var allFields = new[] { "ImageUrl", "Name", "SlugUrl", "Route", "Duration", "PriceOriginal", "PricePromotion", "RatingScore", "RatingCount", "HighlightReview", "ReviewerName", "Tag", "EarlyPromotion", "GiftPromotion" };
            foreach (var f in allFields)
            {
                if (!record.ContainsKey(f) || record[f] == null)
                    record[f] = "";
            }
            // Chuyển đổi kiểu dữ liệu cho các trường số
            if (record.ContainsKey("RatingScore"))
            {
                if (double.TryParse(record["RatingScore"].ToString().Replace(",", "."), out var score))
                    record["RatingScore"] = score;
                else
                    record["RatingScore"] = null;
            }
            if (record.ContainsKey("RatingCount"))
            {
                var digits = new string(record["RatingCount"].ToString().Where(char.IsDigit).ToArray());
                if (int.TryParse(digits, out var count))
                    record["RatingCount"] = count;
                else
                    record["RatingCount"] = null;
            }
            // Chuyển đổi giá tiền về số (BỎ ĐI, GIỮ NGUYÊN CHUỖI)
            // foreach (var priceField in new[] { "PriceOriginal", "PricePromotion" })
            // {
            //     if (record.ContainsKey(priceField))
            //     {
            //         var digits = new string(record[priceField].ToString().Where(char.IsDigit).ToArray());
            //         if (long.TryParse(digits, out var price))
            //             record[priceField] = price;
            //         else
            //             record[priceField] = null;
            //     }
            // }
            if (!string.IsNullOrWhiteSpace(record["ImageUrl"].ToString()))
            {
                // Gửi từng bản ghi một lên backend
                var jsonData = JsonConvert.SerializeObject(record, Formatting.Indented);
                // Console.WriteLine("\n===== DỮ LIỆU GỬI LÊN BACKEND (TỪNG BẢN GHI) =====");
                // Console.WriteLine(jsonData);
                // Console.WriteLine("===== HẾT DỮ LIỆU =====\n");
                // Console.WriteLine("📤 Gửi dữ liệu về backend...");
                using var httpClient = new HttpClient();
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(config.BackendApi, content).Result;
                Console.WriteLine($"📬 Kết quả gửi: {response.StatusCode}");
            }
            else
            {
                Console.WriteLine("[SKIP] Bỏ qua bản ghi vì thiếu ImageUrl!");
            }
            // Console.WriteLine("----------------------"); // Bỏ log phân cách từng bản ghi
        }
    }
}
