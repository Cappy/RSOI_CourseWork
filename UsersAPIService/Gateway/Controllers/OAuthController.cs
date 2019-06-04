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

namespace Gateway.Controllers
{
    [Route("api/o/")]
    public class OAuthController : Controller
    {
        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        private const string client_id = "8zyERnIoMHBr1i25cBEantNXoSEJvDOQNoIHTos8";
        private const string client_secret = "DazZrzBTJwZNa2nTkojv6Kceczp3kLDGBBvAzht9GwXEoDXF6IinHEOaTaVEemVPCJTSorrS8df0sMk1DpRtrKOsL89hqrThl8sjjGK7hhQVGHHIG32HNhFceqYqARMQ";
        private const string accessToken = "u6EGsWiNgMVEGEgKgGwUemG0q6JDne";

        public partial class OAuthResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
            public string refresh_token { get; set; }
        }

        public partial class IntrospectResponse
        {
            public bool active { get; set; }
            public string scope { get; set; }
            public int exp { get; set; }
            public string client_id { get; set; }
            public string username { get; set; }
        }

        [HttpPost("/oauth2/authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]OAuthUser oUser)
        {
            HttpResponseMessage user;
            try
            {
                user = await client.PostAsJsonAsync(services.authAPI + "/" + "token"
                    + "/?username=" + oUser.username + "&password=" + oUser.password +
                    "&client_id=" + client_id + "&client_secret=" + client_secret + "&grant_type=password", oUser);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (!user.IsSuccessStatusCode)
            {
                //var message = user.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest();
            }

            var User = await user.Content.ReadAsAsync<OAuthResponse>();

            return Ok(
                new
                {
                    Username = oUser.username,
                    access_token = User.access_token,
                    expires_in = User.expires_in,
                    token_type = User.token_type,
                    scope = User.scope,
                    refresh_token = User.refresh_token
                });
        }

        [HttpGet("introspect")]
        public async Task<IActionResult> Introspect(string token)
        {
            HttpResponseMessage user;

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            if (token == null) token = "123456abcde";

            try
            {
                var stringContent = new StringContent(string.Empty);
                string url = string.Format(services.authAPI + "/" + "introspect"
                    + "/?token={0}", token);
                user = await client.GetAsync(url);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var User = await user.Content.ReadAsAsync<IntrospectResponse>();

            if (user.IsSuccessStatusCode)
            {
                return Ok(User);
            }
            else if (user.StatusCode.ToString() == "Unauthorized")
            {
                return Unauthorized();
            }
            else
            {
                return Forbid();
            }
        }

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


    }
}
