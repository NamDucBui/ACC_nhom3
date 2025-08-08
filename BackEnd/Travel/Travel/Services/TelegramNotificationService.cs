using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Travel.Services
{
    public class TelegramNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramNotificationService> _logger;

        public TelegramNotificationService(HttpClient httpClient, IConfiguration configuration, ILogger<TelegramNotificationService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendNotificationAsync(string message)
        {
            try
            {
                var botToken = _configuration["Telegram:BotToken"];
                var chatId = _configuration["Telegram:ChatId"];

                if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId))
                {
                    _logger.LogWarning("Telegram configuration is missing. BotToken or ChatId not configured.");
                    return;
                }

                var url = $"https://api.telegram.org/bot{botToken}/sendMessage";
                var payload = new
                {
                    chat_id = chatId,
                    text = message,
                    parse_mode = "HTML"
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Telegram notification sent successfully");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send Telegram notification. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending Telegram notification: {ex.Message}");
            }
        }

        public async Task SendCrawlStartNotificationAsync(string configName, string sourceUrl)
        {
            var message = $"🚀 <b>Bắt đầu Crawl</b>\n\n" +
                         $"📋 <b>Cấu hình:</b> {configName}\n" +
                         $"🔗 <b>URL:</b> {sourceUrl}\n" +
                         $"⏰ <b>Thời gian:</b> {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await SendNotificationAsync(message);
        }

        public async Task SendCrawlCompleteNotificationAsync(string configName, bool success, string resultMessage, int toursFound, int toursSaved, int toursSkipped, int pagesCrawled, TimeSpan duration)
        {
            var statusIcon = success ? "✅" : "❌";
            var statusText = success ? "Thành công" : "Thất bại";

            var message = $"{statusIcon} <b>Crawl {statusText}</b>\n\n" +
                         $"📋 <b>Cấu hình:</b> {configName}\n" +
                         $"📊 <b>Kết quả:</b>\n" +
                         $"   • Tìm thấy: {toursFound}\n" +
                         $"   • Lưu: {toursSaved}\n" +
                         $"   • Bỏ qua: {toursSkipped}\n" +
                         $"   • Trang crawl: {pagesCrawled}\n" +
                         $"⏱️ <b>Thời gian:</b> {duration.TotalMinutes:F1} phút\n" +
                         $"⏰ <b>Hoàn thành:</b> {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            if (!success)
            {
                message += $"\n\n❌ <b>Lỗi:</b> {resultMessage}";
            }

            await SendNotificationAsync(message);
        }
    }
} 