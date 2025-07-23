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
    public class UserPlansController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UserPlansController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/UserPlans
        [Authorize]
        [HttpGet]
        public ActionResult<List<UserPlan>> GetUserPlans()
        {
          if (_context.UserPlans == null)
          {
              return NotFound();
          }
            return _context.UserPlans.AsNoTracking().ToList();
        }

        // GET: api/UserPlans/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<UserPlan> GetUserPlan(long id)
        {
          if (_context.UserPlans == null)
          {
              return NotFound();
          }
            var userPlan = _context.UserPlans.Find(id);

            if (userPlan == null)
            {
                return NotFound();
            }

            return userPlan;
        }

        // PUT: api/UserPlans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPut("{id}")]
        public ActionResult PutUserPlan(long id, UserPlan userPlan)
        {
            if (id != userPlan.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userPlan).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserPlanExists(id))
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

        // POST: api/UserPlans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPost]
        public void PostUserPlan(string eMail, short planId)
        {
            Plan plan = _context.Plans.Find(planId);

            UserPlan userPlan = new UserPlan();
            userPlan.PlanId = planId;
            _context.UserPlans.Add(userPlan);
            _context.SaveChanges();
          
        }

        // DELETE: api/UserPlans/5
        [HttpDelete("{id}")]
        public ActionResult DeleteUserPlan(long id)
        {
            if (_context.UserPlans == null)
            {
                return NotFound();
            }
            var userPlan = _context.UserPlans.Find(id);
            if (userPlan == null)
            {
                return NotFound();
            }

            _context.UserPlans.Remove(userPlan);
             _context.SaveChanges();

            return NoContent();
        }

        private bool UserPlanExists(long id)
        {
            return (_context.UserPlans?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
