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
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using Gateway.RetryLogic;

namespace Gateway.Controllers
{
    [Route("api/")]
    public class UsersController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();
        public AuthController AC = new AuthController();


        public string GetTokenFromHeader(HttpRequest request)
        {
            var headers = request.Headers;
            if (headers != null)
            {
                StringValues headerValues;
                if (headers.TryGetValue("Authorization", out headerValues))
                {
                    return headerValues.FirstOrDefault().Substring(7);
                }
            }
            return "";
        }

        //public async Task<bool> ValidateToken()
        //{
        //    var token = GetTokenFromHeader(Request);
        //    HttpResponseMessage introspect;
        //    try
        //    {
        //        var stringContent = new StringContent(string.Empty);
        //        string url = string.Format("http://localhost:4314/api/o/introspect/?token={0}", token);
        //        introspect = await client.GetAsync(url);
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    var Introspect = await introspect.Content.ReadAsAsync<OAuthController.IntrospectResponse>();
        //    if (Introspect == null)
        //    {
        //        return false;
        //    }
        //    else if (Introspect.active == true)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}


        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(int? page, int? size)
        {
            //bool checktoken = await ValidateToken();
            //if(!checktoken)
            //{
            //    return Unauthorized();
            //}

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));

            HttpResponseMessage users;

            try
            {
                users = await client.GetAsync(services.usersAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (users.IsSuccessStatusCode)
            {
                var Users = await users.Content.ReadAsAsync<List<Users>>();
                return Ok(Users);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));

            //bool checktoken = await ValidateToken();
            //if (!checktoken)
            //{
            //    return Unauthorized();
            //}

            HttpResponseMessage user;

            try
            {
                user = await client.GetAsync(services.usersAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (user.IsSuccessStatusCode)
            {
                var User = await user.Content.ReadAsAsync<List<Users>>();
                return Ok(User);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> PutCustomer(Guid id, [FromBody] Users userModel)
        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));
            //bool checktoken = await ValidateToken();
            //if (!checktoken)
            //{
            //    return Unauthorized();
            //}

            HttpResponseMessage customer;

            try
            {
                customer = await client.PutAsJsonAsync(services.usersAPI + $"/{id}", userModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("users")]
        public async Task<IActionResult> PostCustomer([FromBody] Users userModel)
        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));
            //bool checktoken = await ValidateToken();
            //if (!checktoken)
            //{
            //    return Unauthorized();
            //}

            HttpResponseMessage customer;

            Guid id = Guid.NewGuid();
            userModel.Userid = id;

            try
            {
                customer = await client.PostAsJsonAsync(services.usersAPI, userModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(UsersAPI + $"/{customerModel.CustomerId}"));
            return Ok(userModel);

        }

        //удаляет покупателя и все его брони
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AC.GetTokenFromHeader(Request));
            //bool checktoken = await ValidateToken();
            //if (!checktoken)
            //{
            //    return Unauthorized();
            //}

            HttpResponseMessage customer;

            try
            {
                customer = await client.DeleteAsync(services.usersAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null)
            {
                return NotFound();
            }

            HttpResponseMessage bookings = null;

            try
            {
                bookings = await client.GetAsync(services.bookingsAPI);
            }
            catch { }

            if (bookings == null)
            {
                Retry.RetryUntilSuccess(async () =>
                {
                    try
                    {
                        bookings = await client.GetAsync(services.bookingsAPI);
                        if (bookings.IsSuccessStatusCode)
                        {
                            try
                            {
                                var bkR = await bookings.Content.ReadAsAsync<List<Booking>>();

                                foreach (Booking entr in bkR)
                                {
                                    if (entr.Userid == id)
                                    {

                                        HttpResponseMessage booking = await client.DeleteAsync(services.bookingsAPI + $"/{entr.Bookingid}");

                                    }
                                }
                            }
                            catch { return false; }

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch { }
                    return false;
                });

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = $"Booking service is offline"
                });
            }


            var bk = await bookings.Content.ReadAsAsync<List<Booking>>();

            try
            {
                foreach (Booking entr in bk)
                {
                    if (entr.Userid == id)
                    {

                        HttpResponseMessage booking = await client.DeleteAsync(services.bookingsAPI + $"/{entr.Bookingid}");

                    }
                }
            }
            catch { }

            return Ok(customer);
        }
    }
}
