using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Identity;
using OKN.Core.Models.Queries;
using OKN.WebApp.Models.Objects;
using OKN.Core.Models.Commands;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsController : BaseController
    {
        private readonly ObjectsRepository _objectsRepository;

        public ObjectsController(ObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        // POST api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
        [HttpPost("{objectId}")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Update([FromRoute]string objectId,
                                                [FromBody] UpdateObjectViewModel request)
        {
            if (request == null) return BadRequest();

            var objectQuery = new ObjectQuery(objectId);

            var current = await _objectsRepository.GetObject(objectQuery, CancellationToken.None);

            if (current == null) return NotFound();

            var currentUser = HttpContext.User;

            var updateCommand = new UpdateObjectCommand(new ObjectId(objectId))
            {
                ObjectId = objectId,
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Type = request.Type,

                UserId = long.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserName = currentUser.FindFirstValue(ClaimTypes.Name),
                Email = currentUser.FindFirstValue(ClaimTypes.Email)
            };

            await _objectsRepository.UpdateObject(updateCommand, CancellationToken.None);

            return Ok();
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5
        [HttpGet("{objectId}")]
        [ProducesResponseType(typeof(OknObject), 200)]
        public async Task<IActionResult> Get([FromRoute]string objectId)
        {
            var model = await _objectsRepository.GetObject(new ObjectQuery(objectId), CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        // GET api/objects
        [HttpGet]
        [ProducesResponseType(typeof(List<OknObject>), 200)]
        public async Task<IActionResult> List([FromQuery]int? page,
                                              [FromQuery]int? perPage, [FromQuery] string types)
        {
            var query = new ListObjectsQuery
            {
                Page = page ?? 1,
                PerPage = perPage ?? DefaultPerPage
            };

            if (types != null)
            {
                query.Types = types.Split(',').Select(x => (EObjectType)(int.Parse(x))).ToArray();
            }

            var model = await _objectsRepository.GetObjects(query, CancellationToken.None);
            if (model == null)
                return BadRequest();

            return Ok(model);
        }
    }
}
