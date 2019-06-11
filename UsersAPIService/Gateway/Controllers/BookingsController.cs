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
            HttpResponseMessage bookingsOfUser = null;
            HttpResponseMessage bookingsOfAd = null;

            if(bookingModel.ArrivalDate == null || bookingModel.DepartureDate == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err = "Invalid date."
                });
            }

            if (bookingModel.ArrivalDate >= bookingModel.DepartureDate)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err = "Departure date can't be less than arrival date."
                });
            }

            if (bookingModel.ArrivalDate < DateTime.Now)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err = "Arrival date can't be less than current date."
                });
            }

            if (bookingModel.ArrivalDate > DateTime.Today.AddYears(2))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err = "Maximum arrival date for booking is current date plus two years. Maximum arrival date for today is: " + DateTime.Today.AddYears(2).Date
                });
            }

            if (bookingModel.DepartureDate > DateTime.Today.AddYears(3))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err = "Maximum departure date for booking is current date plus three years. Maximum departure date for today is: " + DateTime.Today.AddYears(3).Date
                });
            }


            try
            {
                ad = await client.GetAsync(services.adsAPI + $"/{bookingModel.Adid}");
                user = await client.GetAsync(services.usersAPI + $"/{bookingModel.Userid}");
                bookingsOfUser = await client.GetAsync(services.gatewayAPI + $"/users/{bookingModel.Userid}/bookings");
                bookingsOfAd = await client.GetAsync(services.gatewayAPI + $"/ads/{bookingModel.Adid}/bookings");
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            List<Booking> BookingsOfUser = await bookingsOfUser.Content.ReadAsAsync<List<Booking>>();
            List<Booking> BookingsOfAd = await bookingsOfAd.Content.ReadAsAsync<List<Booking>>();

            foreach (var entry in BookingsOfUser)
            {
                if (entry.Adid == bookingModel.Adid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        err = "You may have only one booking of any housing.\n" +
                        "Before making a new book, please, cancel old booking."
                    });
                }
            }

            foreach (var entry in BookingsOfAd)
            {
                if ((bookingModel.ArrivalDate <= entry.ArrivalDate && entry.ArrivalDate <= bookingModel.DepartureDate && bookingModel.DepartureDate <= entry.DepartureDate && bookingModel.ArrivalDate <= entry.DepartureDate) ||
                    (entry.ArrivalDate <= bookingModel.ArrivalDate && bookingModel.ArrivalDate <= entry.DepartureDate && bookingModel.DepartureDate <= entry.DepartureDate && bookingModel.DepartureDate >= entry.ArrivalDate) ||
                    (entry.ArrivalDate <= bookingModel.ArrivalDate && bookingModel.ArrivalDate <= entry.DepartureDate && entry.DepartureDate <= bookingModel.DepartureDate && entry.ArrivalDate <= bookingModel.DepartureDate) ||
                    (bookingModel.ArrivalDate<=entry.ArrivalDate && entry.ArrivalDate<=bookingModel.DepartureDate && entry.DepartureDate <= bookingModel.DepartureDate && bookingModel.ArrivalDate <= entry.DepartureDate))
                    {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        err = "These dates are busy.\n" +
                        "Look into booked dates from the prevoius page and choose new dates."
                    });
                }
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

            if (booking.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var message = booking.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest(new { err = message });

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