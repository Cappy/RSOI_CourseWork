using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Gateway.Controllers;

namespace Gateway.Controllers
{
    [Route("api/")]
    public class BookingsController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();
        public AuthController AC = new AuthController();

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings(int? page, int? size)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));


            string bookings;
            try
            {
                bookings = await client.GetStringAsync(services.bookingsAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (bookings == null)
            {
                return NotFound();
            }

            var Bookings = JsonConvert.DeserializeObject<List<Booking>>(bookings);

            return Ok(Bookings);
        }

        [HttpGet("bookings/{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));
            string booking;
            try
            {
                booking = await client.GetStringAsync(services.bookingsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            var Booking = JsonConvert.DeserializeObject<Booking>(booking);

            return Ok(Booking);
        }

        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> PutBooking(Guid id, [FromBody] Booking bookingModel)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));
            HttpResponseMessage booking;
            try
            {
                booking = await client.PutAsJsonAsync(services.bookingsAPI + $"/{id}", bookingModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{id}"));
            return Ok();
        }

        [HttpPost("bookings")]
        public async Task<IActionResult> PostBooking([FromBody] Booking bookingModel)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));
            HttpResponseMessage ad = null;
            HttpResponseMessage user = null;

            try
            {
                ad = await client.GetAsync(services.adsAPI + $"/{bookingModel.Adid}");
                user = await client.GetAsync(services.usersAPI + $"/{bookingModel.Userid}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (ad == null || user == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            HttpResponseMessage booking;

            Guid id = Guid.NewGuid();
            bookingModel.Bookingid = id;

            try
            {
                booking = await client.PostAsJsonAsync(services.bookingsAPI, bookingModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{customerModel.CustomerId}"));
            return Ok(bookingModel);

        }

        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));
            HttpResponseMessage booking;
            try
            {
                booking = await client.DeleteAsync(services.bookingsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

    }
}