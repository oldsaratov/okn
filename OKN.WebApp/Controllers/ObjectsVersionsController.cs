﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OKN.Core.Models.Queries;
using OKN.Core.Repositories;

namespace OKN.WebApp.Controllers
{
    [Route("api/objects")]
    public class ObjectsVersionsController : BaseController
    {
        private readonly ObjectsRepository _objectsRepository;

        public ObjectsVersionsController(ObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/versions
        /// <summary>
        /// List object versions by object Id
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        [HttpGet("{objectId}/versions")]
        [ProducesResponseType(typeof(List<VersionInfo>), 200)]
        public async Task<IActionResult> ListVersions([FromQuery]int? page,
            [FromQuery]int? perPage, [FromRoute]string objectId)
        {
            var model = await _objectsRepository.GetVersions(new ListVersionsQuery(objectId, page, perPage), CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        // GET api/objects/2abbbeb2-baba-4278-9ad4-2c275aa2a8f5/versions/1
        /// <summary>
        /// Get object version by object Id and version Id
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet("{objectId}/versions/{version}")]
        [ProducesResponseType(typeof(OknObject), 200)]
        public async Task<IActionResult> GetVersion([FromRoute]string objectId, [FromRoute] long version)
        {
            var model = await _objectsRepository.GetObject(new ObjectQuery(objectId, version), CancellationToken.None);

            if (model == null)
                return NotFound();

            return Ok(model);
        }
    }
}
