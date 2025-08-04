using Microsoft.AspNetCore.Mvc;
using Travel.Services;

namespace Travel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlController : ControllerBase
    {
        private readonly CrawlerService _crawlerService;
        private readonly ILogger<CrawlController> _logger;

        public CrawlController(CrawlerService crawlerService, ILogger<CrawlController> logger)
        {
            _crawlerService = crawlerService;
            _logger = logger;
        }

        // POST: api/Crawl/run/{configId}
        [HttpPost("run/{configId}")]
        public async Task<IActionResult> RunCrawl(int configId)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu crawl với config ID: {configId}");
                
                var result = await _crawlerService.RunCrawlWithConfigAsync(configId);
                
                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        tourCount = result.TourCount
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi crawl: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        // GET: api/Crawl/status
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                service = "Crawler Service",
                status = "Running",
                timestamp = DateTime.Now
            });
        }
    }
} 