using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Travel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public HotelsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetAll()
        {
            var hotels = await _context.Hotels.ToListAsync();
            return Ok(hotels);
        }
    }
} 