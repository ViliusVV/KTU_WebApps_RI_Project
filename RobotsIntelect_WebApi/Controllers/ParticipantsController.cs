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
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IMongoRepository<Participant> _participantRepository;

        public ParticipantsController(IMongoRepository<Participant> participantRepository)
        {
            _participantRepository = participantRepository;
        }


        /// <summary>
        /// Gets all participants
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<Participant>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetList()
        {
            var people = _participantRepository.AsQueryable().ToList();

            if (people.Count() > 0)
            {
                return Ok(people);
            }

            return NoContent();
        }


        /// <summary>
        /// Get participant by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Participant), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult Get(string id)
        {
            var participant = _participantRepository.FindById(id);

            if(participant != null)
            {
                return Ok(participant);
            }

            return NotFound("Entry not found");
        }


        /// <summary>
        /// Create participant
        /// </summary>
        /// <param name="participant"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Participant), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] Participant participant)
        {
            if(participant == null)
            {
                return BadRequest("Empty request body");
            }

            _participantRepository.InsertOne(participant);

            var path = $"{Request.Path}/{participant.Id.ToString()}";

            return Created(path, participant);
        }


        /// <summary>
        /// Update participant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Participant), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult Update(string id, [FromBody] Participant participant)
        {
            if(participant == null)
            {
                return BadRequest("Empty request body!");
            }

            if(!_participantRepository.Exists(id))
            {
                return NotFound("Entry not found");
            }

            participant.Id = new ObjectId(id);
            _participantRepository.ReplaceOne(participant);

            return Ok(participant);
        }


        /// <summary>
        /// Delete participant by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Participant), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult Delete(string id)
        {
            var participant = _participantRepository.FindById(id);

            if (participant == null)
            {
                return NotFound("Entry not found");
            }

            _participantRepository.DeleteById(id);

            return Ok(participant);
        }

    }
}
