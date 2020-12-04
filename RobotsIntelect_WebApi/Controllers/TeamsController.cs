using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RobotsIntelect_WebApi.Models;
using RobotsIntelect_WebApi.Repository.Interfaces;
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
    public class TeamsController : ControllerBase
    {
        private readonly IMongoRepository<Team> _teamsRepository;

        public TeamsController(IMongoRepository<Team> teamsRepository)
        {
            _teamsRepository = teamsRepository;
        }


        /// <summary>
        /// Gets all teams.
        /// All user can access this endpoint.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetList()
        {
            var people = _teamsRepository.AsQueryable().ToList();

            if (people.Count() > 0)
            {
                return Ok(people);
            }

            return NoContent();
        }


        /// <summary>
        /// Get team by id.
        /// All user can access this endpoint.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult Get(string id)
        {
            var user = _teamsRepository.FindById(id);

            if(user != null)
            {
                return Ok(user);
            }

            return NotFound("Entry not found");
        }


        /// <summary>
        /// Create team.
        /// Only Admin or referee can acces this endpoint.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.AdminOrReferee)]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Create([FromBody] Team user)
        {
            if(user == null)
            {
                return BadRequest("Empty request body");
            }

            _teamsRepository.InsertOne(user);

            var path = $"{Request.Path}/{user.Id.ToString()}";

            return Created(path, user);
        }


        /// <summary>
        /// Update team.
        /// Only Admin or referee can acces this endpoint.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.AdminOrReferee)]
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Update(string id, [FromBody] Team user)
        {
            if(user == null)
            {
                return BadRequest("Empty request body!");
            }

            if(!_teamsRepository.Exists(id))
            {
                return NotFound("Entry not found");
            }

            user.Id = new ObjectId(id);
            _teamsRepository.ReplaceOne(user);

            return Ok(user);
        }


        /// <summary>
        /// Delete team by id.
        /// Only Admin or referee can acces this endpoint.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.AdminOrReferee)]
        [HttpDelete("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Delete(string id)
        {
            var user = _teamsRepository.FindById(id);

            if (user == null)
            {
                return NotFound("Entry not found");
            }

            _teamsRepository.DeleteById(id);

            return Ok(user);
        }

    }
}
