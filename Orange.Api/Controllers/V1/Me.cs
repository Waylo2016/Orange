using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.Guild;

namespace Orange.Api.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Tags("Me")]
[Produces("application/json")]
public class MeController : ControllerBase
{
    /// <summary>
    /// invoked wanting to see the guilds the current dashboard user has access to
    /// </summary>
    /// <remarks>
    /// Auth: dashboard only (not yet enforced - see TODO.md).
    /// TODO: no dashboard identity/session concept exists yet, so there is no "current user" to resolve. See TODO.md.
    /// </remarks>
    /// <returns>the guilds belonging to the current dashboard user</returns>
    [HttpGet("guilds")]
    [ProducesResponseType(typeof(List<GuildSummaryDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<List<GuildSummaryDto>>> GetMyGuilds()
    {
        return Task.FromResult<ActionResult<List<GuildSummaryDto>>>(StatusCode(StatusCodes.Status501NotImplemented));
    }
}
