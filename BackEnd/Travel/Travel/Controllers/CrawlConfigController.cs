using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;

namespace Travel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlConfigController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CrawlConfigController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CrawlConfig
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CrawlConfig>>> GetCrawlConfigs()
        {
            return await _context.CrawlConfigs
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // GET: api/CrawlConfig/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CrawlConfig>> GetCrawlConfig(int id)
        {
            var crawlConfig = await _context.CrawlConfigs.FindAsync(id);

            if (crawlConfig == null)
            {
                return NotFound();
            }

            return crawlConfig;
        }

        // POST: api/CrawlConfig
        [HttpPost]
        public async Task<ActionResult<CrawlConfig>> PostCrawlConfig(CrawlConfig crawlConfig)
        {
            if (ModelState.IsValid)
            {
                crawlConfig.CreatedAt = DateTime.Now;
                _context.CrawlConfigs.Add(crawlConfig);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCrawlConfig), new { id = crawlConfig.Id }, crawlConfig);
            }

            return BadRequest(ModelState);
        }

        // POST: api/CrawlConfig/create-bestprice-sample
        [HttpPost("create-bestprice-sample")]
        public async Task<ActionResult<CrawlConfig>> CreateBestPriceSample()
        {
            var sampleConfig = new CrawlConfig
            {
                SourceName = "BestPrice - Tour Nhật Bản",
                SourceBaseUrl = "https://www.bestprice.vn/tour/nhat-ban",
                SourceContainer = ".tour-list",
                SourceEachItemContainer = ".tour-item",
                SourceLinkQueryParams = ".tour-item a",
                SourceAvatarQueryParams = ".tour-item img",
                SourceTitleQueryParams = ".tour-item .tour-title",
                SourceSapoQueryParams = ".tour-item .tour-route",
                SourceDurationQueryParams = ".tour-item .tour-duration",
                SourcePriceOriginalQueryParams = ".tour-item .tour-price-original",
                SourcePricePromotionQueryParams = ".tour-item .tour-price-promotion",
                SourceRatingScoreQueryParams = ".tour-item .tour-rating-score",
                SourceRatingCountQueryParams = ".tour-item .tour-rating-count",
                BackendApi = "http://localhost:5298/api/Tour",
                HasLoadMore = true,
                LoadMoreButtonSelector = ".btn-more-tour",
                MaxLoadMore = 5,
                IsActive = true
            };

            sampleConfig.CreatedAt = DateTime.Now;
            _context.CrawlConfigs.Add(sampleConfig);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCrawlConfig), new { id = sampleConfig.Id }, sampleConfig);
        }

        // PUT: api/CrawlConfig/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrawlConfig(int id, CrawlConfig crawlConfig)
        {
            if (id != crawlConfig.Id)
            {
                return BadRequest();
            }

            var existingConfig = await _context.CrawlConfigs.FindAsync(id);
            if (existingConfig == null)
            {
                return NotFound();
            }

            existingConfig.SourceName = crawlConfig.SourceName;
            existingConfig.SourceBaseUrl = crawlConfig.SourceBaseUrl;
            existingConfig.SourceContainer = crawlConfig.SourceContainer;
            existingConfig.SourceEachItemContainer = crawlConfig.SourceEachItemContainer;
            existingConfig.SourceLinkQueryParams = crawlConfig.SourceLinkQueryParams;
            existingConfig.SourceAvatarQueryParams = crawlConfig.SourceAvatarQueryParams;
            existingConfig.SourceTitleQueryParams = crawlConfig.SourceTitleQueryParams;
            existingConfig.SourceSapoQueryParams = crawlConfig.SourceSapoQueryParams;
            existingConfig.SourceDurationQueryParams = crawlConfig.SourceDurationQueryParams;
            existingConfig.SourcePriceOriginalQueryParams = crawlConfig.SourcePriceOriginalQueryParams;
            existingConfig.SourcePricePromotionQueryParams = crawlConfig.SourcePricePromotionQueryParams;
            existingConfig.SourceRatingScoreQueryParams = crawlConfig.SourceRatingScoreQueryParams;
            existingConfig.SourceRatingCountQueryParams = crawlConfig.SourceRatingCountQueryParams;

            existingConfig.BackendApi = crawlConfig.BackendApi;
            existingConfig.HasLoadMore = crawlConfig.HasLoadMore;
            existingConfig.LoadMoreButtonSelector = crawlConfig.LoadMoreButtonSelector;
            existingConfig.MaxLoadMore = crawlConfig.MaxLoadMore;
            existingConfig.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CrawlConfigExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CrawlConfig/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrawlConfig(int id)
        {
            var crawlConfig = await _context.CrawlConfigs.FindAsync(id);
            if (crawlConfig == null)
            {
                return NotFound();
            }

            // Soft delete - chỉ đánh dấu không active
            crawlConfig.IsActive = false;
            crawlConfig.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/CrawlConfig/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<CrawlConfig>>> GetActiveCrawlConfigs()
        {
            return await _context.CrawlConfigs
                .Where(c => c.IsActive)
                .OrderBy(c => c.SourceName)
                .ToListAsync();
        }

        private bool CrawlConfigExists(int id)
        {
            return _context.CrawlConfigs.Any(e => e.Id == id);
        }
    }
} 