using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RobotsIntelect_WebApi.Models;
using RobotsIntelect_WebApi.Models.Auth;
using RobotsIntelect_WebApi.Repository.Interfaces;
using RobotsIntelect_WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoRepository<User> _userRepository;
        private IAuthService _authService;

        public AuthController(IMongoRepository<User> userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = _authService.Authenticate(model, IpAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _authService.RefreshToken(refreshToken, IpAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("RevokeToken")]
        public IActionResult RevokeToken([FromForm] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = _authService.RevokeToken(token, IpAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("{id}/RefreshTokens")]
        public IActionResult GetRefreshTokens(string id)
        {
            var user = _authService.GetById(id);
            if (user == null) return NotFound();

            return Ok(user.RefreshTokens);
        }

        // Helper methods

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}
