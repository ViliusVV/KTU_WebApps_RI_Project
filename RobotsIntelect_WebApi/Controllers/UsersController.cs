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
    public class UsersController : ControllerBase
    {
        private readonly IMongoRepository<User> _userRepository;
        private IAuthService _authService;

        public UsersController(IMongoRepository<User> userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }


        /// <summary>
        /// Gets all users.
        /// Only admins can acces this endpoint.
        /// </summary>
        /// <returns>List of users</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetList()
        {
            var people = _userRepository.AsQueryable().ToList();

            if (people.Count() > 0)
            {
                return Ok(people);
            }

            return NoContent();
        }


        /// <summary>
        /// Get user by id.
        /// Only admins can acces this endpoint.
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns>Found user</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get(string id)
        {
            var user = _userRepository.FindById(id);

            if(user != null)
            {
                return Ok(user);
            }

            return NotFound("Entry not found");
        }


        /// <summary>
        /// Create user.
        /// Only admins can acces this endpoint.
        /// </summary>
        /// <param name="user">New user</param>
        /// <returns>Created user with resource path</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Create([FromBody] User user)
        {
            if(user == null)
            {
                return BadRequest("Empty request body");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _userRepository.InsertOne(user);

            var path = $"{Request.Path}/{user.Id.ToString()}";

            return Created(path, user);
        }


        /// <summary>
        /// Update user.
        /// Only admins can acces this endpoint.
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <param name="user">New updated user</param>
        /// <returns>Updated user</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Update(string id, [FromBody] User user)
        {
            if(user == null)
            {
                return BadRequest("Empty request body!");
            }

            if(!_userRepository.Exists(id))
            {
                return NotFound("Entry not found");
            }

            var oldUser = _userRepository.FindById(id);
            if (user.PasswordHash == null || user.PasswordHash.Length == 0)
            {
                user.PasswordHash = oldUser.PasswordHash;
            }
            else
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }

            user.Id = new ObjectId(id);
            _userRepository.ReplaceOne(user);

            return Ok(user);
        }


        /// <summary>
        /// Delete user by ID.
        /// Only admins can acces this endpoint.
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns>Deleted user</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Delete(string id)
        {
            var user = _userRepository.FindById(id);

            if (user == null)
            {
                return NotFound("Entry not found");
            }

            _userRepository.DeleteById(id);

            return Ok(user);
        }
    }
}
