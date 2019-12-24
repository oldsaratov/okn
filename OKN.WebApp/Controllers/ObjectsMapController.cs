using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;
using OKN.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Models.Queries;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsMapController : BaseController
    {
        private readonly IQueryProcessor _queryProcessor;

        private const int DefaultZoomLevel = 1;

        public ObjectsMapController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }


        // GET api/objects/map
        /// <summary>
        /// Get object by search params and bbox
        /// </summary>
        /// <param name="types"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        [HttpGet("map")]
        [ProducesResponseType(typeof(List<OknObject>), 200)]
        public async Task<IActionResult> GetMap([FromQuery] string types, int? zoomLevel)
        {
            var query = new ListObjectsForMapQuery
            {
                ZoomLevel = zoomLevel ?? DefaultZoomLevel
            };

            if (types != null)
            {
                query.Types = types.Split(',').Select(x => (EObjectType)(int.Parse(x))).ToArray();
            }

            var model = await _queryProcessor.ProcessAsync(query, CancellationToken.None);
            if (model == null)
                return BadRequest();

            return Ok(model);
        }
    }
}
