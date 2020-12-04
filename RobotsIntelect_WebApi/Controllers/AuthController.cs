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


        /// <summary>
        /// Authenticate/login user, responds with token on successfull authentication. 
        /// All users can access this endpoint.
        /// </summary>
        /// <param name="model">Authentication info needed for login.</param>
        /// <returns>JWT token and refresh token.</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = _authService.Authenticate(model, IpAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }


        /// <summary>
        /// Authenticates user using refresh token.
        /// All users can access this endpoint.
        /// </summary>
        /// <returns>Returns new JWT and refresh tokens</returns>
        [AllowAnonymous]
        [HttpPost("refreshToken")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _authService.AuthenticateSensor(refreshToken, IpAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }



        /// <summary>
        /// Authenticates sensor using api key (refresh token)
        /// All users can access this endpoint.
        /// </summary>
        /// <returns>Returns new JWT and refresh tokens</returns>
        [AllowAnonymous]
        [HttpPost("sensorAuth")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult SensorAuthentication()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _authService.AuthenticateSensor(refreshToken, IpAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }


        /// <summary>
        /// Revokes/invalidates refresh token. 
        /// Only authenticated users can accesss this endpoint.
        /// </summary>
        /// <param name="model">Token to revoke</param>
        /// <returns>Confirmation/failure message</returns>
        [Authorize]
        [HttpPost("revokeToken")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token;

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = _authService.RevokeToken(token, IpAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }


        /// <summary>
        /// Revokes/invalidates currently used refresh token. 
        /// Only authenticated users can accesss this endpoint.
        /// </summary>
        /// <param name="model">Token to revoke</param>
        /// <returns>Confirmation/failure message</returns>
        [Authorize]
        [HttpPost("revokeCurrentToken")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult RevokeCurrentTokenToken()
        {
            var token =  Request.Cookies["refreshToken"];
            return RevokeToken(new RevokeTokenRequest() { Token = token});
        }

        /// <summary>
        /// List all stored refresh tokens.
        /// Only authenticated users can accesss this endpoint.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}/refreshTokens")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(List<RefreshToken>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult GetRefreshTokens(string id)
        {
            var user = _authService.GetById(id);
            if (user == null) return NotFound();

            if (user.RefreshTokens.Count == 0) return NoContent();

            return Ok(user.RefreshTokens);
        }


        // ======== Helper methods =========

        // Set refresh cookie
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        // Get client/request IP
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}
