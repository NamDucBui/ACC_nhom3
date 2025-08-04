using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;

namespace Travel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TourController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tour
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            return await _context.Tours.Include(t => t.CrawlConfig).ToListAsync();
        }

        // GET: api/Tour/by-config/{configId}
        [HttpGet("by-config/{configId}")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetToursByConfig(int configId)
        {
            var tours = await _context.Tours
                .Where(t => t.CrawlConfigId == configId)
                .Include(t => t.CrawlConfig)
                .ToListAsync();
            
            return tours;
        }

        // GET: api/Tour/without-config
        [HttpGet("without-config")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetToursWithoutConfig()
        {
            var tours = await _context.Tours
                .Where(t => t.CrawlConfigId == null)
                .ToListAsync();
            
            return tours;
        }

        // GET: api/Tour/configs
        [HttpGet("configs")]
        public async Task<ActionResult<object>> GetToursByConfigs()
        {
            var result = await _context.CrawlConfigs
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    ConfigId = c.Id,
                    ConfigName = c.SourceName,
                    TourCount = _context.Tours.Count(t => t.CrawlConfigId == c.Id),
                    LastCrawled = _context.Tours
                        .Where(t => t.CrawlConfigId == c.Id)
                        .OrderByDescending(t => t.Id)
                        .Select(t => t.Id)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return result;
        }

        // GET: api/Tour/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
                return NotFound();
            return tour;
        }

        // POST: api/Tour
        [HttpPost]
        public async Task<ActionResult<Tour>> PostTour(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, tour);
        }

        // PUT: api/Tour/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTour(int id, Tour tour)
        {
            if (id != tour.Id)
                return BadRequest();
            _context.Entry(tour).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tours.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        // DELETE: api/Tour/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
                return NotFound();
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
} 