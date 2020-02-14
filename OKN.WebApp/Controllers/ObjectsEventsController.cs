using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Models;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Queries;
using OKN.Core.Repositories;
using OKN.WebApp.Models.ObjectEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsEventsController : BaseController
    {
        private readonly ObjectsEventRepository _objectsEventsRepository;

        public ObjectsEventsController(ObjectsEventRepository objectsEventsRepository)
        {
            _objectsEventsRepository = objectsEventsRepository;
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

            var updateCommand = new CreateObjectEventCommand(objectId, Guid.NewGuid().ToString())
            {
                Name = request.Name,
                Description = request.Description,
                OccuredAt = request.OccuredAt ?? DateTime.UtcNow,
                Files = request.Files?.Select(x => new FileInfo
                {
                    FileId = x.FileId,
                    Description = x.Description
                }).ToList(),
                Photos = request.Photos?.Select(x => new FileInfo
                {
                    FileId = x.FileId,
                    Description = x.Description
                }).ToList()
            };

            updateCommand.SetCreator(
                long.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier)).ToString(),
                currentUser.FindFirstValue(ClaimTypes.Name),
                currentUser.FindFirstValue(ClaimTypes.Email));

            await _objectsEventsRepository.CreateObjectEvent(updateCommand, CancellationToken.None);

            return Ok();
        }

        // POST api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/events/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
        /// <summary>
        /// Update event for object
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="eventId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{objectId}/events/{eventId}")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Update([FromRoute]string objectId, [FromRoute]string eventId,
            [FromBody] UpdateObjectEventViewModel request)
        {
            if (request == null)
                return BadRequest();

            var currentUser = HttpContext.User;

            var updateCommand = new UpdateObjectEventCommand(objectId, eventId)
            {
                Name = request.Name,
                Description = request.Description,
                OccuredAt = request.OccuredAt ?? DateTime.UtcNow,
                Files = request.Files?.Select(x => new FileInfo
                {
                    FileId = x.FileId,
                    Description = x.Description
                }).ToList(),
                Photos = request.Photos?.Select(x => new FileInfo
                {
                    FileId = x.FileId,
                    Description = x.Description
                }).ToList()
            };

            updateCommand.SetCreator(
                long.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier)).ToString(),
                currentUser.FindFirstValue(ClaimTypes.Name),
                currentUser.FindFirstValue(ClaimTypes.Email));

            await _objectsEventsRepository.UpdateObjectEvent(updateCommand, CancellationToken.None);

            return Ok();
        }


        // DELETE api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/events/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
        /// <summary>
        /// Delete event of object
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpDelete("{objectId}/events/{eventId}")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Delete([FromRoute]string objectId, [FromRoute]string eventId)
        {
            var currentUser = HttpContext.User;

            var deleteCommand = new DeleteObjectEventCommand(objectId, eventId);

            deleteCommand.SetCreator(
                long.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier)).ToString(),
                currentUser.FindFirstValue(ClaimTypes.Name),
                currentUser.FindFirstValue(ClaimTypes.Email));

            await _objectsEventsRepository.DeleteObjectEvent(deleteCommand, CancellationToken.None);

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
        [ProducesResponseType(typeof(List<OknObjectEvent>), 200)]
        public async Task<IActionResult> ListEvents([FromQuery]int? page,
            [FromQuery]int? perPage, [FromRoute]string objectId)
        {
            var model = await _objectsEventsRepository.GetObjectEvents(new ListObjectEventsQuery(objectId, page, perPage), CancellationToken.None);

            if (model == null)
                return Ok(Array.Empty<OknObjectEvent>());

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
        [ProducesResponseType(typeof(OknObjectEvent), 200)]
        public async Task<IActionResult> GetEvent([FromRoute]string objectId, [FromRoute] string eventId)
        {
            var model = await _objectsEventsRepository.GetEvent(new ObjectEventQuery(objectId, eventId), CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }
    }
}
