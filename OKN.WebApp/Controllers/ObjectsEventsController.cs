using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Queries;
using Microsoft.AspNetCore.Authorization;
using OKN.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Identity;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Queries;
using OKN.WebApp.Models.ObjectEvents;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsEventsController : BaseController
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryProcessor _queryProcessor;

        public ObjectsEventsController(ICommandBus commandBus, IQueryProcessor queryProcessor)
        {
            _commandBus = commandBus;
            _queryProcessor = queryProcessor;
        }

        // POST api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/events
        [HttpPost("{objectId}/events")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Create([FromRoute]string objectId,
            [FromBody] CreateObjectEventViewModel request)
        {
            if (request == null) return BadRequest();

            var currentUser = HttpContext.User;

            var links = request.Links?.Select(x => new OKNObjectEventLink(x.Description, x.Url)).ToList();
            var images = request.Images?.Select(x => new OKNObjectEventImage(x.Description, x.Url)).ToList();

            var updateCommand = new CreateObjectEventCommand(new ObjectId(objectId))
            {
                ObjectId = objectId,
                EventId = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                OccuredAt = request.OccuredAt ?? DateTime.UtcNow,
                Links = links,
                Images = images,

                UserId = long.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserName = currentUser.FindFirstValue(ClaimTypes.Name),
                Email = currentUser.FindFirstValue(ClaimTypes.Email)
            };

            await _commandBus.PublishAsync(updateCommand, CancellationToken.None);

            return Ok();
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/events
        [HttpGet("{objectId}/events")]
        [ProducesResponseType(typeof(List<OKNObjectEvent>), 200)]
        public async Task<IActionResult> ListEvents([FromQuery]int? page,
            [FromQuery]int? perPage, [FromRoute]string objectId)
        {
            var model = await _queryProcessor.ProcessAsync(new ListObjectEventsQuery
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
        [HttpGet("{objectId}/events/{eventId}")]
        [ProducesResponseType(typeof(OKNObjectEvent), 200)]
        public async Task<IActionResult> GetEvent([FromRoute]string objectId, [FromRoute] string eventId)
        {
            var model = await _queryProcessor.ProcessAsync(new ObjectEventQuery(objectId, eventId), CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }
    }
}
