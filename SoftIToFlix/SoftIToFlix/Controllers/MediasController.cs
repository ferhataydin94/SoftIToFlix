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
    public class MediasController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public MediasController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Medias
        [Authorize]
        [HttpGet]
        public ActionResult<List<Media>> GetMedias()
        {
          if (_context.Medias == null)
          {
              return NotFound();
          }
            return _context.Medias.Where(m=>m.Passive==false).AsNoTracking().ToList();
        }

        // GET: api/Medias/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Media>> GetMedia(int id)
        {
          if (_context.Medias == null)
          {
              return NotFound();
          }
            var media = await _context.Medias.FindAsync(id);

            if (media == null || media.Passive==true)
            {
                return NotFound();
            }

            return media;
        }

        // PUT: api/Medias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPut("{id}")]
        public ActionResult PutMedia(int id, Media media)
        {
            if (id != media.Id)
            {
                return BadRequest();
            }

            _context.Entry(media).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaExists(id))
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

        // POST: api/Medias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPost]
        public ActionResult<Media> PostMedia(Media media)
        {
          if (_context.Medias == null)
          {
              return Problem("Entity set 'ApplicationContext.Medias'  is null.");
          }
            _context.Medias.Add(media);
            _context.SaveChanges();

            return CreatedAtAction("GetMedia", new { id = media.Id }, media);
        }

        // DELETE: api/Medias/5
        [Authorize(Roles = "ContentAdmin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteMedia(int id)
        {
            if (_context.Medias == null)
            {
                return NotFound();
            }
            var media =  _context.Medias.FindAsync(id).Result;
            if (media == null || media.Passive==true)
            {
                return NotFound();
            }
            media.Passive = true;
            _context.Medias.Update(media);

            IQueryable episodes = _context.Episodes.Where(e => e.MediaId == media.Id);

            foreach(Episode episode in episodes)
            {
                episode.Passive = true;
                _context.Episodes.Update(episode);
            }



            _context.SaveChanges();

            return NoContent();
        }

        private bool MediaExists(int id)
        {
            return (_context.Medias?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
