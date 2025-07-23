using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class EpisodesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public EpisodesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Episodes
        [Authorize]
        [HttpGet]
        public ActionResult<List<Episode>> GetEpisodes(int mediaId, byte seaseonNumber)
        {
          if (_context.Episodes == null)
          {
              return NotFound();
          }
            return _context.Episodes.Where(e=>e.MediaId==mediaId && e.SeasonNumber== seaseonNumber).OrderBy(e=>e.EpisodeNumber).AsNoTracking().ToList();
        }

        // GET: api/Episodes/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Episode> GetEpisode(long id)
        {
          if (_context.Episodes == null)
          {
              return NotFound();
          }
            var episode = _context.Episodes.Find(id);

            if (episode == null)
            {
                return NotFound();
            }


            return episode;
        }

        [HttpGet("Watch")]
        [Authorize]
        public void Watch(long id)
        {
             
            UserWatched userWatched = new UserWatched();
            Episode episode = _context.Episodes.Find(id);
            try { 
           
            userWatched.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            
            
            userWatched.EpisodeId = id;
            _context.UserWatcheds?.Add(userWatched);
                episode.ViewCount ++;
                _context.Episodes?.Update(episode);
            _context.SaveChanges();
            }


            catch(Exception ex)
            {

            }
        }

        // PUT: api/Episodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPut("{id}")]
        public ActionResult PutEpisode(long id, Episode episode)
        {
            if (id != episode.Id)
            {
                return BadRequest();
            }
           // Episode episode1 = _context.Episodes.Where(e => e.Id == episode.Id).FirstOrDefault();
            _context.Entry(episode).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeExists(id))
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

        // POST: api/Episodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPost]
        public ActionResult<Episode> PostEpisode(Episode episode)
        {
          if (_context.Episodes == null)
          {
              return Problem("Entity set 'ApplicationContext.Episodes'  is null.");
          }
            _context.Episodes.Add(episode);
            _context.SaveChanges();

            return CreatedAtAction("GetEpisode", new { id = episode.Id }, episode);
        }

        // DELETE: api/Episodes/5
        [Authorize(Roles = "ContentAdmin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteEpisode(long id)
        {
            if (_context.Episodes == null)
            {
                return NotFound();
            }
            var episode = _context.Episodes.Find(id);
            if (episode == null)
            {
                return NotFound();
            }
            episode.Passive = true;
            _context.Episodes.Update(episode);
            _context.SaveChanges();

            return NoContent();
        }

        private bool EpisodeExists(long id)
        {
            return (_context.Episodes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
