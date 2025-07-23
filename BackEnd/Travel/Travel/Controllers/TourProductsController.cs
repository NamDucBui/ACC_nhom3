using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Travel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TourProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TourProduct>>> GetAll()
        {
            var tours = await _context.TourProducts.ToListAsync();
            return Ok(tours);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TourProduct>> GetById(int id)
        {
            var tour = await _context.TourProducts.FindAsync(id);
            if (tour == null) return NotFound();
            return Ok(tour);
        }

        [HttpPost]
        public async Task<ActionResult<TourProduct>> Create(TourProduct tourProduct)
        {
            _context.TourProducts.Add(tourProduct);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = tourProduct.Id }, tourProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TourProduct tourProduct)
        {
            if (id != tourProduct.Id) return BadRequest();
            _context.Entry(tourProduct).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TourProducts.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _context.TourProducts.FindAsync(id);
            if (tour == null) return NotFound();
            _context.TourProducts.Remove(tour);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
} 