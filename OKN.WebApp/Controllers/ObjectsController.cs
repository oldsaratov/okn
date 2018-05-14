using System;
using System.Net;
using System.Threading.Tasks;
using OKN.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Queries;
using OKN.WebApp.Models.Objects;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsController : BaseController
    {
        private readonly IMediator _mediator;

        public ObjectsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
        [HttpPost("{objectId}"), Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OKNObject))]
        public async Task<IActionResult> Update([FromRoute]string objectId,
                                                [FromBody] UpdateObjectViewModel request)
        {
            if (request == null) return BadRequest();
            
            var current = await _mediator.Send(new ObjectQuery()
            {
                ObjectId = objectId
            });

            if (current == null) return NotFound();
            
            var currentUser = HttpContext.User;
                
            await _mediator.Send(new UpdateObjectCommand()
            {
                ObjectId = objectId,
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Type = request.Type
            });
            
            var model = await _mediator.Send(new ObjectQuery()
            {
                ObjectId = objectId
            });

            return Ok(model);
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
        [HttpGet("{objectId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OKNObject))]
        public async Task<IActionResult> Get([FromRoute]string objectId, [FromQuery] long? version)
        {
            var model = await _mediator.Send(new ObjectQuery()
            {
                ObjectId = objectId,
                Version = version
            });

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        // GET api/objects
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PagedList<OKNObject>))]
        public async Task<IActionResult> List([FromQuery]int? page,
                                              [FromQuery]int? perPage)
        {
            var model = await _mediator.Send(new ListObjectsQuery
            {
                Page = page ?? 1,
                PerPage = perPage ?? DEFAULT_PER_PAGE
            });

            if (model == null)
                return BadRequest();

            return Ok(model);
        }
    }
}
