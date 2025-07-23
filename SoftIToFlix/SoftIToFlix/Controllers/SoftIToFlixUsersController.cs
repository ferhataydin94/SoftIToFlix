using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using SoftIToFlix.Data;
using SoftIToFlix.Models;

namespace SoftIToFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoftIToFlixController : ControllerBase
    {
        public struct LogInModel
        {
            public string UserName { get; set; }
           public string Password { get; set; }

        }

        private readonly SignInManager<SoftIToFlixUser> _signInManager;
        private readonly ApplicationContext _context;

        public SoftIToFlixController(SignInManager<SoftIToFlixUser> signInManager, ApplicationContext context)
        {

            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/SoftIToFlix
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult<List<SoftIToFlixUser>> GetUsers(bool includePassive = true)
        {
            IQueryable<SoftIToFlixUser> users = _signInManager.UserManager.Users;
            if (users == null)
            {
                return NotFound();
            }

            if (includePassive == false)
            {
                users = users.Where(u => u.Passive == false);
            }

            // users.Where(u => u.UserName.StartsWith("a") == true);
            return users.AsNoTracking().ToList();

        }

        // GET: api/SoftIToFlix/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<SoftIToFlixUser> GetSoftIToFlixUser(long id)
        {
            SoftIToFlixUser? softIToFlixUser = null;
            if (User.IsInRole("CustomerRepresentative") == false)

            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized();
                }

            }

            softIToFlixUser = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();


            if (softIToFlixUser == null || softIToFlixUser.Passive == true)
            {
                return NotFound();
            }

            return softIToFlixUser;
        }

        // PUT: api/SoftIToFlix/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public ActionResult PutSoftIToFlixUser(SoftIToFlixUser softIToFlixUser)
        {

            SoftIToFlixUser? user = _signInManager.UserManager.Users.Where(u => u.Id == softIToFlixUser.Id).FirstOrDefault();
            //_context.Entry(softIToFlixUser).State = EntityState.Modified;
            if (user == null)
            {
                return NotFound();
            }
            user.PhoneNumber = softIToFlixUser.PhoneNumber;
            user.UserName = softIToFlixUser.UserName;
            user.BirthDate = softIToFlixUser.BirthDate;
            user.Email = softIToFlixUser.Email;
            user.Name = softIToFlixUser.Name;

            _signInManager.UserManager.UpdateAsync(user).Wait();

            return Ok();
        }

        // POST: api/SoftIToFlix
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        public ActionResult<string> PostSoftIToFlixUser(SoftIToFlixUser softIToFlixUser)
        {
            if (User.Identity!.IsAuthenticated == true)
            {
                return BadRequest();
            }
            IdentityResult identityResult = _signInManager.UserManager.CreateAsync(softIToFlixUser, softIToFlixUser.Password).Result;

            if (identityResult != IdentityResult.Success)
            {

                return identityResult.Errors.FirstOrDefault()!.Description;
            }

            return Ok();
        }

        // DELETE: api/SoftIToFlix/5
        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteSoftIToFlixUser(long id)
        {
            SoftIToFlixUser? user = null;
            if (User.IsInRole("CustomerRepresentative") == false)

            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized();
                }

            }
            user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            user.Passive = true;
            _signInManager?.UserManager.UpdateAsync(user).Wait();
            return Ok();
        }



        private bool SoftIToFlixUserExists(long id)
        {
            return (_signInManager.UserManager.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost("Login")]
        public ActionResult<List<Media>> Login(LogInModel loginModel)
        {
            Microsoft.AspNetCore.Identity.SignInResult result;
            SoftIToFlixUser user = _signInManager.UserManager.FindByNameAsync(loginModel.UserName).Result;
            List<Media> medias = new List<Media>();
            List<UserFavorite> userFavorites;
            IQueryable<Media> mediaQuery;
            IQueryable<int> userWatches;
            IGrouping<short, MediaCategory> mediaCategories;

            if (user == null)
            {
                return NotFound();
            }

            if (_context.UserPlans.Where(u => u.UserId == user.Id && u.EndDate >= DateTime.Today).Any() == false)
            {
                user.Passive = true;
                _signInManager.UserManager.UpdateAsync(user).Wait();
            }

            if (user.Passive == true)
            {
                return Content("Passive");
            }
            result = _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false).Result;


            if (result.Succeeded == true)
            {
                mediaCategories = _context.UserFavorites.Where(u => u.UserId == user.Id).
                   Include(u => u.Media).
                   ThenInclude(u => u.MediaCategories).
                   SelectMany(u => u.Media!.MediaCategories!).
                   GroupBy(m => m.CategoryId).
                   OrderByDescending(m => m.Count()).
                   FirstOrDefault()!;
                if (mediaCategories != null)
                {
                    userWatches = _context.UserWatcheds.Where(u => u.UserId == user.Id).Include(u => u.Episode).Select(u => u.Episode!.MediaId).Distinct();

                    mediaQuery = _context.Medias.Include(m => m.MediaCategories.Where(mc => mc.CategoryId == mediaCategories.Key)).Where(m => userWatches.Contains(m.Id));
                    if(user.Restriction != null)
                    {
                        mediaQuery= mediaQuery.Include(m=>m.MediaRestrictions!.Where(r=>r.RestrictionId <= user.Restriction));
                    }
                    medias = mediaQuery.ToList();
                }
            }
            return medias;
        }



        [Authorize]
        [HttpGet("LogOut")]
        public void LogOut()
        {
            _signInManager.SignOutAsync().Wait();

        }


      

    }
}