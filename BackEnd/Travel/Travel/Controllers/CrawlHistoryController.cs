using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;

namespace Travel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CrawlHistoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CrawlHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CrawlHistory>>> GetCrawlHistories()
        {
            return await _context.CrawlHistories
                .Include(ch => ch.CrawlConfig)
                .OrderByDescending(ch => ch.StartedAt)
                .ToListAsync();
        }

        // GET: api/CrawlHistory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CrawlHistory>> GetCrawlHistory(int id)
        {
            var crawlHistory = await _context.CrawlHistories
                .Include(ch => ch.CrawlConfig)
                .FirstOrDefaultAsync(ch => ch.Id == id);

            if (crawlHistory == null)
            {
                return NotFound();
            }

            return crawlHistory;
        }

        // GET: api/CrawlHistory/config/5
        [HttpGet("config/{configId}")]
        public async Task<ActionResult<IEnumerable<CrawlHistory>>> GetCrawlHistoriesByConfig(int configId)
        {
            return await _context.CrawlHistories
                .Include(ch => ch.CrawlConfig)
                .Where(ch => ch.CrawlConfigId == configId)
                .OrderByDescending(ch => ch.StartedAt)
                .ToListAsync();
        }

        // POST: api/CrawlHistory
        [HttpPost]
        public async Task<ActionResult<CrawlHistory>> PostCrawlHistory(CrawlHistory crawlHistory)
        {
            _context.CrawlHistories.Add(crawlHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCrawlHistory), new { id = crawlHistory.Id }, crawlHistory);
        }

        // PUT: api/CrawlHistory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrawlHistory(int id, CrawlHistory crawlHistory)
        {
            if (id != crawlHistory.Id)
            {
                return BadRequest();
            }

            _context.Entry(crawlHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CrawlHistoryExists(id))
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

        // DELETE: api/CrawlHistory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrawlHistory(int id)
        {
            var crawlHistory = await _context.CrawlHistories.FindAsync(id);
            if (crawlHistory == null)
            {
                return NotFound();
            }

            _context.CrawlHistories.Remove(crawlHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CrawlHistoryExists(int id)
        {
            return _context.CrawlHistories.Any(e => e.Id == id);
        }
    }
} 