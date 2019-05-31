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
using System.Net;

namespace Gateway.Controllers
{
    [Route("api/")]
    public class AdsController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        [HttpGet("ads")]
        public async Task<IActionResult> GetAds(int? page, int? size)
        {
            HttpResponseMessage ads;
            try
            {
                ads = await client.GetAsync(services.adsAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (ads == null)
            {
                return NotFound();
            }

            List<Ads> Ads = null;

            if (ads.IsSuccessStatusCode)
            {
                Ads = await ads.Content.ReadAsAsync<List<Ads>>();
            }


            return Ok(Ads);
        }

        [HttpGet("ads/{id}")]
        public async Task<IActionResult> GetAd(Guid id)
        {

            HttpResponseMessage ad;
            try
            {
                ad = await client.GetAsync(services.adsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (ad == null)
            {
                return NotFound();
            }

            Ads Ad = null;

            if (ad.IsSuccessStatusCode)
            {
                Ad = await ad.Content.ReadAsAsync<Ads>();
            }

            return Ok(Ad);
        }

        [HttpPut("ads/{id}")]
        public async Task<IActionResult> PutAd(Guid id, [FromBody] Ads adModel)
        {
            string err = string.Empty;

            if (adModel.Bathrooms <= 0)
            {
                err += "Number of bathrooms must be greater than 0. ";
            }
            if (adModel.Beds <= 0)
            {
                err += "Number of beds must be greater than 0. ";
            }

            if (adModel.Price <= 0)
            {
                err += "Price must be greater than 0.";
            }

            if (err != "")
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err
                });
            }

            HttpResponseMessage ad;
            try
            {
                ad = await client.PutAsJsonAsync(services.adsAPI + $"/{id}", adModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (ad == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("ads")]
        public async Task<IActionResult> PostAd([FromBody] Ads adModel)
        {
            HttpResponseMessage ad = null;
            string err = string.Empty;

            Guid id = Guid.NewGuid();
            adModel.Adid = id;


            if (adModel.Bathrooms <= 0)
            {
                err += "Number of bathrooms must be greater than 0. ";
            }
            if (adModel.Beds <= 0)
            {
                err += "Number of beds must be greater than 0. ";
            }
            if (adModel.Price <= 0)
            {
                err += "Price must be greater than 0.";
            }

            if (err != "")
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err
                });
            }

            try
            {
                ad = await client.PostAsJsonAsync(services.adsAPI, adModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (ad == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{customerModel.CustomerId}"));
            return Ok(adModel);

        }

        [HttpDelete("ads/{id}")]
        public async Task<IActionResult> DeleteAd(Guid id)
        {

            Ads AdBU = new Ads();
            HttpResponseMessage adBackup = null;

            //getting room info before deleteing
            try
            {
                adBackup = await client.GetAsync(services.gatewayAPI + $"/ads/{id}");
                AdBU = await adBackup.Content.ReadAsAsync<Ads>();
            }
            catch
            {
                if (adBackup.StatusCode != HttpStatusCode.ServiceUnavailable)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        err = string.Format("Ad with ID {0} is not found in the DB, nothing to delete", id)
                    });
                }
            }


            HttpResponseMessage ad;
            try
            {
                ad = await client.DeleteAsync(services.adsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (ad == null)
            {
                return NotFound();
            }

            HttpResponseMessage bookings = null;

            try
            {
                bookings = await client.GetAsync(services.bookingsAPI);
            }
            catch
            {
                var restoreAd = await client.PostAsJsonAsync(services.gatewayAPI + "/ads", AdBU);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Rolled back: Booking service is unavailable."
                });

            }

            var bk = await bookings.Content.ReadAsAsync<List<Booking>>();

            try
            {
                foreach (Booking entr in bk)
                {
                    if (entr.Adid == id)
                    {

                        HttpResponseMessage booking = await client.DeleteAsync(services.bookingsAPI + $"/{entr.Bookingid}");

                    }
                }
            }
            catch
            {
                var restoreAd = await client.PostAsJsonAsync(services.gatewayAPI + "/ads", AdBU);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Rolled back: Booking service is unavailable."
                });
            }

            return Ok(ad);
        }

    }
}
