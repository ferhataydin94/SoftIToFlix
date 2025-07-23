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
    public class DirectorsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public DirectorsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Directors
        [Authorize]
        [HttpGet]
        public ActionResult<List<Director>> GetDirectors()
        {
          if (_context.Directors == null)
          {
              return NotFound();
          }
            return _context.Directors.Where(d=> d.Passive==false).AsNoTracking().ToList();
        }

        // GET: api/Directors/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Director> GetDirector(int id)
        {
          if (_context.Directors == null)
          {
              return NotFound();
          }
            var director = _context.Directors.Find(id);

            if (director == null)
            {
                return NotFound();
            }

            return director;
        }

        // PUT: api/Directors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPut("{id}")]
        public void  PutDirector(Director director)
        {
            

            _context.Entry(director).State = EntityState.Modified;

            try
            {
               _context.SaveChanges();
            }
            catch (Exception ex)
            {
                
            }

           
        }

        // POST: api/Directors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPost]
        public int PostDirector(Director director)
        {
          
            _context.Directors.Add(director);
            _context.SaveChanges();

            //return CreatedAtAction("GetDirector", new { id = director.Id }, director);
            return director.Id;
        }

        // DELETE: api/Directors/5
        [Authorize(Roles = "ContentAdmin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteDirector(int id)
        {
            if (_context.Directors == null)
            {
                return NotFound();
            }
            var director = _context.Directors.FindAsync(id).Result;
            if (director == null)
            {
                return NotFound();
            }
            director.Passive = true;
            _context.Directors.Update(director);
             _context.SaveChanges();

            return NoContent();
        }

        private bool DirectorExists(int id)
        {
            return (_context.Directors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
