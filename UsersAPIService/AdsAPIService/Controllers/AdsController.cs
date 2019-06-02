using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdsAPIService.Models;
using PagedList;
using Microsoft.Extensions.Logging;

namespace AdsAPIService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AdsController : Controller
    {
        private readonly AdsContext _context;

        public AdsController(AdsContext context)
        {
            _context = context;
        }

        // GET: api/Ads
        [HttpGet]
        public IEnumerable<Ads> GetAds(int? page, int? size)
        {

            if (page == null || size == null)
            {
                return _context.Ads;
            }

            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 1);

            return _context.Ads.ToPagedList(pageNumber, pageSize).Distinct()
            .OrderBy(d => d.City).ThenByDescending(d => d.CreatedAt);
        }

        // GET: api/Ads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAds([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Ads = await _context.Ads.SingleOrDefaultAsync(m => Convert.ToString(m.Adid) == id.ToString());

            if (Ads == null)
            {
                return NotFound();
            }

            return Ok(Ads);
        }

        // PUT: api/Ads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAds([FromRoute] Guid id, [FromBody] Ads Ads)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Ads.Adid)
            {
                return BadRequest();
            }

            _context.Entry(Ads).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdsExists(id))
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

        // POST: api/Ads
        [HttpPost]
        public async Task<IActionResult> PostAds([FromBody] Ads Ads)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ads.CreatedAt = DateTime.Now;

            _context.Ads.Add(Ads);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdsExists(Ads.Adid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAds", new { id = Ads.Adid }, Ads);
        }

        // DELETE: api/Ads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAds([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Ads = await _context.Ads.SingleOrDefaultAsync(m => Convert.ToString(m.Adid) == id.ToString());
            if (Ads == null)
            {
                return NotFound();
            }

            _context.Ads.Remove(Ads);
            await _context.SaveChangesAsync();

            return Ok(Ads);
        }

        private bool AdsExists(Guid id)
        {
            return _context.Ads.Any(m => Convert.ToString(m.Adid) == id.ToString());
        }
    }
}