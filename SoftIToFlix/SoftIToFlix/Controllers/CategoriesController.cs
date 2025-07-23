using System;
using System.Collections.Generic;
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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [Authorize]
        [HttpGet]
        public ActionResult<List<Category>>GetCategories()
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            return  _context.Categories.AsNoTracking().ToList();
        }

        // GET: api/Categories/5
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(short id)
        {
          
            Category? category =  _context.Categories?.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPut("{id}")]
        public void PutCategory( Category category)
        {

           // _context.Categories?.Update(category);

           _context.Entry(category).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (Exception ex)
            {
                
            }

            
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "ContentAdmin")]
        [HttpPost]
        public short PostCategory(Category category)
        {
          
            _context.Categories?.Add(category);
            _context.SaveChanges();

            // return CreatedAtAction("GetCategory", new { id = category.Id }, category);

            return category.Id;
        }

        
       

        private bool CategoryExists(short id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
