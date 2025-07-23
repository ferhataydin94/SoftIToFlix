using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftIToFlix.Data;
using SoftIToFlix.Models;

namespace SoftIToFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StarsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public StarsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Stars
        [Authorize]
        [HttpGet]
        public ActionResult<List<Star>> GetStars()
        {
          if (_context.Stars == null)
          {
              return NotFound();
          }
            return _context.Stars.AsNoTracking().ToList();
        }

        // GET: api/Stars/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Star> GetStar(int id)
        {
          if (_context.Stars == null)
          {
              return NotFound();
          }
            var star =  _context.Stars.Find(id);

            if (star == null)
            {
                return NotFound();
            }

            return star;
        }

        // PUT: api/Stars/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPut("{id}")]
        public ActionResult PutStar(int id, Star star)
        {
            if (id != star.Id)
            {
                return BadRequest();
            }

            _context.Entry(star).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StarExists(id))
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

        // POST: api/Stars
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPost]
        public ActionResult<Star> PostStar(Star star)
        {
          if (_context.Stars == null)
          {
              return Problem("Entity set 'ApplicationContext.Stars'  is null.");
          }
            _context.Stars.Add(star);
             _context.SaveChanges();

            return CreatedAtAction("GetStar", new { id = star.Id }, star);
        }

        // DELETE: api/Stars/5
       
        private bool StarExists(int id)
        {
            return (_context.Stars?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
