using System;
using System.Net;
using System.Threading.Tasks;
using OKN.Core.Models;
using MediatR;
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

		// POST api/objects
		[HttpPost]
		[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
		public async Task<IActionResult> Create()
		{
			var fileId = await _mediator.Send(new CreateObjectCommand());

			if (fileId == null)
			  return NotFound();

			return Ok(fileId);
		}

		// POST api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/update
		[HttpPost("{objectId}")]
		[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OKNObject))]
        public async Task<IActionResult> Update([FromRoute]string objectId, 
                                                [FromBody] UpdateObjectViewModel viewModel)
		{
            if (viewModel != null)
            {
                var result = await _mediator.Send(new UpdateObjectCommand()
                {
	                ObjectId = objectId,
                    Name = viewModel.Name,
                    Description = viewModel.Description
                });

                return Ok(result);
            }

            return BadRequest();
		}

		// GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
		[HttpGet("{objectId}")]
		[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OKNObject))]
		public async Task<IActionResult> Get([FromRoute]string objectId)
		{
            var model = await _mediator.Send(new ObjectQuery() 
            { 
                ObjectId = objectId
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
