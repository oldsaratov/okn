using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;
using OKN.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Models.Queries;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsVersionsController : BaseController
    {
        private readonly IQueryProcessor _queryProcessor;

        public ObjectsVersionsController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/versions
        [HttpGet("{objectId}/versions")]
        [ProducesResponseType(typeof(List<VersionInfo>), 200)]
        public async Task<IActionResult> ListVersions([FromQuery]int? page,
            [FromQuery]int? perPage, [FromRoute]string objectId)
        {
            var model = await _queryProcessor.ProcessAsync(new ListVersionsQuery
            {
                ObjectId = objectId,
                Page = page ?? 1,
                PerPage = perPage ?? DefaultPerPage
            }, CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/versions/1
        [HttpGet("{objectId}/versions/{version}")]
        [ProducesResponseType(typeof(OknObject), 200)]
        public async Task<IActionResult> GetVersion([FromRoute]string objectId, [FromRoute] long version)
        {
            var model = await _queryProcessor.ProcessAsync(new ObjectQuery(objectId, version), CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }
    }
}
