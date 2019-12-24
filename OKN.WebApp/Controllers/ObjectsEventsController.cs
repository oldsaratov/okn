﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OKN.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Queries;
using OKN.Core.Repositories;
using OKN.WebApp.Models.ObjectEvents;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsEventsController : BaseController
    {
        private readonly ObjectsRepository _objectsRepository;

        public ObjectsEventsController(ObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        // POST api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/events
        /// <summary>
        /// Create event for object
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{objectId}/events")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Create([FromRoute]string objectId,
            [FromBody] CreateObjectEventViewModel request)
        {
            if (request == null) return BadRequest();

            var currentUser = HttpContext.User;

            var updateCommand = new CreateObjectEventCommand
            {
                ObjectId = objectId,
                EventId = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                OccuredAt = request.OccuredAt ?? DateTime.UtcNow,

                UserId = long.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserName = currentUser.FindFirstValue(ClaimTypes.Name),
                Email = currentUser.FindFirstValue(ClaimTypes.Email)
            };

            await _objectsRepository.CreateEvent(updateCommand, CancellationToken.None);

            return Ok();
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/events
        /// <summary>
        /// List object events
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        [HttpGet("{objectId}/events")]
        [ProducesResponseType(typeof(List<OKNObjectEvent>), 200)]
        public async Task<IActionResult> ListEvents([FromQuery]int? page,
            [FromQuery]int? perPage, [FromRoute]string objectId)
        {
            var model = await _objectsRepository.GetObjectEvents(new ListObjectEventsQuery
            {
                ObjectId = objectId,
                Page = page ?? 1,
                PerPage = perPage ?? DefaultPerPage
            }, CancellationToken.None);

            if (model == null)
                return Ok(Array.Empty<OKNObjectEvent>());

            return Ok(model);
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/events/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
        /// <summary>
        /// Get event by object Id and event Id
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("{objectId}/events/{eventId}")]
        [ProducesResponseType(typeof(OKNObjectEvent), 200)]
        public async Task<IActionResult> GetEvent([FromRoute]string objectId, [FromRoute] string eventId)
        {
            var model = await _objectsRepository.GetEvent(new ObjectEventQuery(objectId, eventId), CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }
    }
}
