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
using System.Net;

namespace Gateway.Controllers
{
    [Route("api/")]
    public class AggregationController : Controller
    {
        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        //all bookings of this customer
        [HttpGet("users/{id}/bookings")]
        public async Task<IActionResult> GetBookingsOfUser(Guid id)
        {
            HttpResponseMessage bookings;
            //Booking bookings;
            //string bookings;
            try
            {
                bookings = await client.GetAsync(services.bookingsAPI + $"/get-bookings-by-userid/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            List<Booking> Bookings = null;

            if (bookings.IsSuccessStatusCode)
            {
                Bookings = await bookings.Content.ReadAsAsync<List<Booking>>();
            }

            if (Bookings == null)
            {
                return NotFound();
            }

            //var bk = Bookings.Where(x => x.Userid == id).ToList();

            return Ok(Bookings);

        }

        [HttpGet("ads/{id}/bookings")]
        public async Task<IActionResult> GetBookingsOfAd(Guid id)
        {
            HttpResponseMessage bookings;
            //Booking bookings;
            //string bookings;
            try
            {
                bookings = await client.GetAsync(services.bookingsAPI + $"/get-bookings-by-adid/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            List<Booking> Bookings = null;

            if (bookings.IsSuccessStatusCode)
            {
                Bookings = await bookings.Content.ReadAsAsync<List<Booking>>();
            }

            if (Bookings == null)
            {
                return NotFound();
            }

            //var bk = Bookings.Where(x => x.Userid == id).ToList();

            return Ok(Bookings);

        }

        [HttpGet("booking-with-info/{id}")]
        public async Task<IActionResult> GetBookingWithInfo(Guid id)
        {
            HttpResponseMessage booking;
            try
            {
                booking = await client.GetAsync(services.bookingsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Booking service is unavailable"
                });
            }

            if (booking.StatusCode == HttpStatusCode.NotFound)
            {
                return StatusCode(StatusCodes.Status404NotFound, new
                {
                    err = string.Format("Booking with ID {0} is not found in the DB", id)
                });
            }

            // var bk = JsonConvert.DeserializeObject<List<Booking>>(booking);
            //Booking bk = JsonConvert.DeserializeObject<Booking>(booking.Content.);
            Booking bk = await booking.Content.ReadAsAsync<Booking>();

            HttpResponseMessage user = null;
            HttpResponseMessage ad = null;

            Users userNull = new Users();
            Ads adNull = new Ads();

            try
            {
                user = await client.GetAsync(services.usersAPI + $"/{bk.Userid}");
            }
            catch
            {
                userNull = new Users()
                {
                    Userid = Guid.Empty,
                    Name = "[service unavailable]",
                    Surname = "",
                    Email = "[service unavailable]"
                };
            }

            try
            {
                ad = await client.GetAsync(services.adsAPI + $"/{bk.Adid}");
            }
            catch
            {

                adNull = new Ads()
                {
                    Adid = Guid.Empty,
                    Userid = Guid.Empty,
                    Caption = "[service unavailable]",
                    City = "[service unavailable]",
                    Adress = "[service unavailable]",
                    Type = "[service unavailable]",
                    WhatRented = "[service unavailable]",
                    Bedrooms = -1,
                    Beds = -1,
                    Bathrooms = -1,
                    Description = "[service unavailable]",
                    Price = -1,
                };
            }

            Users UserInfo = null;
            Ads AdInfo = null;

            if (user.IsSuccessStatusCode)
            {
                UserInfo = await user.Content.ReadAsAsync<Users>();
            }
            else
            {
                UserInfo = userNull;
            }
            if (ad.IsSuccessStatusCode)
            {
                AdInfo = await ad.Content.ReadAsAsync<Ads>();
            }
            else
            {
                AdInfo = adNull;
            }

            var result = new BookingWithInfo
            {
                Bookingid = bk.Bookingid,
                user = UserInfo,
                //new Users
                //{
                //    Userid = UserInfo.Userid,
                //    Name = UserInfo.Name,
                //    Surname = UserInfo.Surname,
                //    Email = UserInfo.Email
                //},
                ad = AdInfo
                //new Ads
                //{
                //    Adid = AdInfo.Adid,
                //    Userid = AdInfo.Userid,
                //    Caption = AdInfo.Caption,
                //    City = AdInfo.,
                //    Adress = "[service unavailable]",
                //    Type = "[service unavailable]",
                //    WhatRented = "[service unavailable]",
                //    Bedrooms = -1,
                //    Beds = -1,
                //    Bathrooms = -1,
                //    Description = "[service unavailable]",
                //    Price = -1,
                //}
            };

            return Ok(result);
        }

        [HttpGet("bookings-with-info")]
        public async Task<IActionResult> GetBookings(int? page, int? size)
        {
            HttpResponseMessage bookings;
            try
            {
                bookings = await client.GetAsync(services.bookingsAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Booking service is unavailable (503) [API message]"
                });
            }

            if (bookings == null)
            {
                return NotFound();
            }

            // var bk = JsonConvert.DeserializeObject<List<Booking>>(booking);
            //var Bookings = JsonConvert.DeserializeObject<List<Booking>>(bookings);

            var Bookings = await bookings.Content.ReadAsAsync<List<Booking>>();

            HttpResponseMessage user = null;
            HttpResponseMessage ad = null;

            var result = new List<BookingWithInfo>();

            Users userNull = new Users();
            Ads adNull = new Ads();

            foreach (Booking bk in Bookings)
            {
                try
                {
                    user = await client.GetAsync(services.usersAPI + $"/{bk.Userid}");
                }
                catch
                {
                    userNull = new Users()
                    {
                        Userid = Guid.Empty,
                        Name = "[service unavailable]",
                        Surname = "",
                        Email = "[service unavailable]"
                    };
                }

                try
                {
                    ad = await client.GetAsync(services.adsAPI + $"/{bk.Adid}");
                }
                catch
                {

                    adNull = new Ads()
                    {
                        Adid = Guid.Empty,
                        Userid = Guid.Empty,
                        Caption = "[service unavailable]",
                        City = "[service unavailable]",
                        Adress = "[service unavailable]",
                        Type = "[service unavailable]",
                        WhatRented = "[service unavailable]",
                        Bedrooms = -1,
                        Beds = -1,
                        Bathrooms = -1,
                        Description = "[service unavailable]",
                        Price = -1,
                    };
                }

                Users UserInfo = null;
                Ads AdInfo = null;


                if (user.IsSuccessStatusCode)
                {
                    UserInfo = await user.Content.ReadAsAsync<Users>();
                }
                else
                {
                    UserInfo = userNull;
                }
                if (ad.IsSuccessStatusCode)
                {
                    AdInfo = await ad.Content.ReadAsAsync<Ads>();
                }
                else
                {
                    AdInfo = adNull;
                }

                var entry = new BookingWithInfo
                {
                    Bookingid = bk.Bookingid,
                    user = UserInfo,
                    //new Users
                    //{
                    //    Userid = UserInfo.Userid,
                    //    Name = UserInfo.Name,
                    //    Surname = UserInfo.Surname,
                    //    Email = UserInfo.Email
                    //},
                    ad = AdInfo
                    //new Ads
                    //{
                    //    Adid = AdInfo.Adid,
                    //    Userid = AdInfo.Userid,
                    //    Caption = AdInfo.Caption,
                    //    City = AdInfo.,
                    //    Adress = "[service unavailable]",
                    //    Type = "[service unavailable]",
                    //    WhatRented = "[service unavailable]",
                    //    Bedrooms = -1,
                    //    Beds = -1,
                    //    Bathrooms = -1,
                    //    Description = "[service unavailable]",
                    //    Price = -1,
                    //}
                };

                result.Add(entry);

            }

            return Ok(result);
        }
    }
}

