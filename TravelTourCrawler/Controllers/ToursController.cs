using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAllTours()
        {
            try
            {
                _logger.LogInformation("Fetching all tours");

                var crawlTasks = _tourCrawlers.Select(c => c.CrawlToursAsync()).ToList();
                await Task.WhenAll(crawlTasks);

                var allTours = crawlTasks.SelectMany(t => t.Result).ToList();

                return Ok(allTours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tours");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("sources")]
        public IActionResult GetAvailableSources()
        {
            return Ok(new[] { "OTrip", "VietnamBooking" });
        }
    }
}