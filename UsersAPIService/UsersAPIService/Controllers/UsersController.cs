using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAPIService.Models;
using PagedList;
using System.Text.RegularExpressions;

namespace UsersAPIServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UsersContext _context;

        public UsersController(UsersContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<Users> GetUsers(int? page, int? size)
        {

            if (page == null || size == null)
            {
                return _context.Users;
            }

            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 1);

            return _context.Users.ToPagedList(pageNumber, pageSize);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _context.Users.SingleOrDefaultAsync(m => Convert.ToString(m.Userid) == id.ToString());

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers([FromRoute] Guid id, [FromBody] Users Users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (!IsPhoneNumber(Users.PhoneNumber))
            //{
            //    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            //}

            if (id != Users.Userid)
            {
                return BadRequest();
            }

            _context.Entry(Users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUsers([FromBody] Users Users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (!IsPhoneNumber(Users.PhoneNumber))
            //{
            //    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            //}

            _context.Users.Add(Users);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UsersExists(Users.Userid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUsers", new { id = Users.Userid }, Users);
        }

        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+[0-9]{11})$").Success;
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Users = await _context.Users.SingleOrDefaultAsync(m => Convert.ToString(m.Userid) == id.ToString());
            if (Users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(Users);
            await _context.SaveChangesAsync();

            return Ok(Users);
        }

        private bool UsersExists(Guid id)
        {
            return _context.Users.Any(m => Convert.ToString(m.Userid) == id.ToString());
        }
    }
}
