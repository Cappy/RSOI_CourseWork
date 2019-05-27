﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using UsersAPIService.Models; // класс Users
using UsersAPIService.Helpers;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace UsersAPIService.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private UsersContext _context;
        private IUserService _userService;
        private IMapper _mapper;

        public AuthController(UsersContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {

            Guid id = Guid.NewGuid();
            userDto.Userid = id;

            // map dto to entity
            var user = _mapper.Map<Users>(userDto);

            try
            {
                // save 
                _userService.Create(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("/token")]
        public async Task Token(string Email, string Password)
        {

            var identity = GetIdentity(Email, Password);
            if (identity == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                userid = identity.Name,
                roles = identity.Claims
                
            };

            // сериализация ответа
            //Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }));
        }

        private ClaimsIdentity GetIdentity(string Email, string Password)
        {
            //Person person = people.FirstOrDefault(x => x.Login == username && x.Password == password);

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == Email);

            if (user != null)
            {


                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, user.Userid.ToString()));

                if (user.IsAdmin)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin"));
                }

                if (user.IsRentlord)
                {
                    if (!user.IsAdmin)
                    {
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Rentlord"));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Rentlord"));
                    }
                }

                if (!user.IsAdmin && !user.IsRentlord)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "User"));
                }

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<Users>(userDto);
            user.Userid = id;

            try
            {
                // save 
                _userService.Update(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}