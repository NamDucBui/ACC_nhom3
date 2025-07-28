using Microsoft.AspNetCore.Mvc;
using TravelTourCrawler.DTO;
using TravelTourCrawler.Models;
using TravelTourCrawler.Services;

namespace TravelTourCrawler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToursController : ControllerBase
    {
        private readonly IEnumerable<ITourCrawler> _tourCrawlers;
        private readonly ILogger<ToursController> _logger;

        public ToursController(
            IEnumerable<ITourCrawler> tourCrawlers,
            ILogger<ToursController> logger)
        {
            _tourCrawlers = tourCrawlers;
            _logger = logger;
        }

        [HttpGet("crawl")]
        public async Task<IActionResult> GetAllTours([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest(new { error = "URL is required" });

            try
            {
                _logger.LogInformation($"User requested crawl from: {url}");

                // Gọi crawler phù hợp (ở đây giả định chỉ có OTripCrawler)
                var otripCrawler = _tourCrawlers.FirstOrDefault(c => c.Source == "OTrip") as OTripCrawler;
                if (otripCrawler == null)
                    return NotFound(new { error = "OTrip crawler not found" });

                var tours = await otripCrawler.CrawlToursAsync(url);
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crawling from custom URL");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("crawl")]
        public async Task<IActionResult> CrawlDynamic([FromBody] CrawlRequestDto request)
        {
            var crawler = _tourCrawlers.FirstOrDefault(c => c.Source == "OTrip");
            if (crawler is OTripCrawler otrip)
            {
                var tours = await otrip.CrawlWithCustomClassesAsync(request);
                return Ok(tours);
            }

            return NotFound("Crawler not found");
        }

        [HttpGet("sources")]
        public IActionResult GetAvailableSources()
        {
            return Ok(new[] { "OTrip", "VietnamBooking" });
        }
    }
}