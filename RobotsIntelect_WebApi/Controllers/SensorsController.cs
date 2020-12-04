using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RobotsIntelect_WebApi.Models;
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
    public class SensorsController : ControllerBase
    {
        private readonly IMongoRepository<LapSensor> _sensorRepository;
        private IAuthService _authService;

        public SensorsController(IMongoRepository<LapSensor> sensorRepository, IAuthService authService)
        {
            _sensorRepository = sensorRepository;
            _authService = authService;
        }


        /// <summary>
        /// Gets all sensors.
        /// Only Admin or referee can acces this endpoint.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<LapSensor>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetList()
        {
            var people = _sensorRepository.AsQueryable().ToList();

            if (people.Count() > 0)
            {
                return Ok(people);
            }

            return NoContent();
        }


        /// <summary>
        /// Get lapSensor by id.
        /// Only Admin or referee can acces this endpoint.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(LapSensor), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get(string id)
        {
            var lapSensor = _sensorRepository.FindById(id);

            if(lapSensor != null)
            {
                return Ok(lapSensor);
            }

            return NotFound("Entry not found");
        }


        /// <summary>
        /// Create lapSensor.
        /// Only Admin or referee can acces this endpoint.
        /// </summary>
        /// <param name="lapSensor"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(LapSensor), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Create([FromBody] LapSensor lapSensor)
        {
            if(lapSensor == null)
            {
                return BadRequest("Empty request body");
            }

            var refreshToken = _authService.generateRefreshToken(IpAddress());

            lapSensor.ApiKey = refreshToken;

            _sensorRepository.InsertOne(lapSensor);

            var path = $"{Request.Path}/{lapSensor.Id.ToString()}";

            return Created(path, lapSensor);
        }


        /// <summary>
        /// Delete lapSensor by id.
        /// Only Admin or referee can acces this endpoint.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(LapSensor), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Delete(string id)
        {
            var lapSensor = _sensorRepository.FindById(id);

            if (lapSensor == null)
            {
                return NotFound("Entry not found");
            }

            _sensorRepository.DeleteById(id);

            return Ok(lapSensor);
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
