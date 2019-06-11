using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingsAPIService.Models;
using PagedList;
using Microsoft.Extensions.Logging;

namespace BookingsAPIService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BookingsController : Controller
    {
        private readonly BookingContext _context;

        public BookingsController(BookingContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public IEnumerable<Booking> GetBookings(int? page, int? size)
        {
            if (page == null || size == null)
            {
                return _context.Booking;
            }

            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 1);

            return _context.Booking.ToPagedList(pageNumber, pageSize).Distinct()
            .OrderByDescending(d => d.CreatedAt).ThenByDescending(d => d.ArrivalDate);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => Convert.ToString(m.Bookingid) == id.ToString());

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // GET bookings by room ID
        [HttpGet("get-bookings-by-adid/{id}")]
        public IEnumerable<Booking> GetByRoomID([FromRoute] Guid id)
        {
            return _context.Booking.Where(m => m.Adid == id).Distinct()
            .OrderByDescending(d => d.CreatedAt).ThenByDescending(d => d.ArrivalDate);
        }

        // GET all bookings of user
        [HttpGet("get-bookings-by-userid/{id}")]
        public IEnumerable<Booking> GetBookingsOfUser([FromRoute] Guid id)
        {
            return _context.Booking.Where(m => m.Userid == id).Distinct()
            .OrderByDescending(d => d.CreatedAt).ThenByDescending(d => d.ArrivalDate);
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking([FromRoute] Guid id, [FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != booking.Bookingid)
            {
                return BadRequest();
            }

            if (booking.Userid == null || booking.Adid == null || booking.Bookingid == null)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
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

        // POST: api/Bookings
        [HttpPost]
        public async Task<IActionResult> PostBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(booking.ArrivalDate>=booking.DepartureDate)
            {
                return BadRequest("Departure date can't be less than arrival date.");
            }

            if (booking.ArrivalDate < DateTime.Now)
            {
                return BadRequest("Arrival date can't be less than current date.");
            }

            if (booking.ArrivalDate > DateTime.Today.AddYears(2))
            {
                return BadRequest("Maximum arrival date for booking is current date plus two years. Maximum arrival date for today is: " + DateTime.Today.AddYears(2).Date);
            }

            booking.CreatedAt = DateTime.Now;


            _context.Booking.Add(booking);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookingExists(booking.Bookingid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBooking", new { id = booking.Bookingid }, booking);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => Convert.ToString(m.Bookingid) == id.ToString());
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }




        private bool BookingExists(Guid id)
        {
            return _context.Booking.Any(m => Convert.ToString(m.Bookingid) == id.ToString());
        }
    }
}