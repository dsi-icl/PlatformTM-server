using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Services.DTOs.Explorer;
using PlatformTM.Services.Services;

namespace PlatformTM.API.Controllers
{
    [Route("queries")]
    public class QueryController : Controller
    {
        private readonly QueryService _queryService;
        public QueryController(QueryService queryService){
            _queryService = queryService;
        }

        [HttpPost]
        public IActionResult SaveQuery([FromBody] CombinedQueryDTO cdto)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            var savedQuery = _queryService.SaveQuery(cdto, userId);

            if (savedQuery != null)
                return new CreatedAtRouteResult("GetSavedQuery", new {queryId = savedQuery.Id.ToString() }, savedQuery);

            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }


        [HttpGet("{queryId}", Name = "GetSavedQuery")]
        public IActionResult GetSavedQuery(string queryId)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            var query = _queryService.GetSavedCombinedQuery(userId, queryId);
            if (query != null)
                return Ok(query);
            return NotFound();
        }

        [HttpGet("new/{projectId}")]
        public IActionResult CreateQueryObject(int projectId)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var query = _queryService.GetNewCqueryForProject(projectId, userId);
            if (query != null)
                return Ok(query);
            return NotFound();
        }

        [HttpGet("projects/{projectId}/queries/browse")]
        public IActionResult GetSavedQueries(int projectId)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            var queries = _queryService.GetProjectSavedQueries(projectId, userId);
            if (queries != null)
                return Ok(queries);
            return NotFound();
        }
    }
}
