using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gateway.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Collections.Specialized;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

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

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDto userDto)
        {
            HttpResponseMessage user;
            try
            {
                user = await client.PostAsJsonAsync(services.authAPI + "/" + "authenticate", userDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (!user.IsSuccessStatusCode)
            {
                var message = user.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest(message);
            }

            var User = await user.Content.ReadAsAsync<CurUser>();

            return Ok(User);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]UserDto userDto)
        {
            HttpResponseMessage user;
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenFromHeader(Request));
                user = await client.PutAsJsonAsync(services.authAPI + "/" + id, userDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (!user.IsSuccessStatusCode)
            {
                var message = user.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest(message);
            }

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
            HttpResponseMessage user;
            try
            {
                user = await client.PostAsJsonAsync(services.authAPI + "/" + "register", userDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (user.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                var message = user.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest(message);
            }

        }

        [HttpPost("get-oauth2-token")]
        public async Task<IActionResult> GetOA2Token([FromBody]CurUser userModel)
        {
            HttpResponseMessage user;
            try
            {
                user = await client.PostAsJsonAsync(services.authAPI + "/" + "get-oauth2-token", userModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (user.IsSuccessStatusCode)
            {
                var User = await user.Content.ReadAsAsync<CurUser>();
                return Ok(new { User.access_token });
            }
            else
            {
                var message = user.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest(message);
            }

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            HttpResponseMessage users;
            try
            {
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenFromHeader(Request));
                users = await client.GetAsync(services.authAPI);

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
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            HttpResponseMessage users;
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenFromHeader(Request));
                users = await client.DeleteAsync(services.authAPI + "/" + id);

                if (users.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

        }


    }
}

