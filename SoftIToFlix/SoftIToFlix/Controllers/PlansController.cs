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
    public class PlansController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PlansController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Plans
        [Authorize]
        [HttpGet]
        public ActionResult<List<Plan>> GetPlans()
        {
          if (_context.Plans == null)
          {
              return NotFound();
          }
            return  _context.Plans.ToList();
        }

        // GET: api/Plans/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Plan> GetPlan(short id)
        {
          if (_context.Plans == null)
          {
              return NotFound();
          }
            var plan = _context.Plans.FindAsync(id).Result;

            if (plan == null)
            {
                return NotFound();
            }

            return plan;
        }

        // PUT: api/Plans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPut("{id}")]
        public ActionResult PutPlan(short id, Plan plan)
        {
            if (id != plan.Id)
            {
                return BadRequest();
            }

            _context.Entry(plan).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(id))
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

        // POST: api/Plans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Plan> PostPlan(Plan plan)
        {
          if (_context.Plans == null)
          {
              return Problem("Entity set 'ApplicationContext.Plans'  is null.");
          }
            _context.Plans.Add(plan);
            _context.SaveChanges();

            return CreatedAtAction("GetPlan", new { id = plan.Id }, plan);
        }

        // DELETE: api/Plans/5
        [HttpDelete("{id}")]
        public ActionResult DeletePlan(short id)
        {
            if (_context.Plans == null)
            {
                return NotFound();
            }
            var plan =  _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            _context.Plans.Remove(plan);
             _context.SaveChanges();

            return NoContent();
        }

        private bool PlanExists(short id)
        {
            return (_context.Plans?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
