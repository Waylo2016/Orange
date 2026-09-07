using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.GuildSettings;

namespace Orange.Api.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/guilds/{guildId:ulong}/settings")]
[ApiVersion("1.0")]
[Tags("Guild - Settings")]
[Produces("application/json")]
public class GuildSettingsController : ControllerBase
{
    /// <summary>
    /// invoked wanting to see the settings for a guild
    /// </summary>
    /// <remarks>
    /// Auth: bot + mod (not yet enforced - see TODO.md).
    /// TODO: no IGuildSettingsService / GuildSettingsService exists yet - see TODO.md.
    /// </remarks>
    /// <param name="guildId">The ID of the guild</param>
    /// <returns>the guild's settings</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GuildSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuildSettingsDto>> GetGuildSettings([FromRoute] ulong guildId)
    {
        return Problem("not yet implemented", statusCode: StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// invoked wanting to update the settings for a guild
    /// </summary>
    /// <remarks>
    /// Auth: bot + mod (not yet enforced - see TODO.md).
    /// TODO: no IGuildSettingsService / GuildSettingsService exists yet - see TODO.md.
    /// </remarks>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="guildSettings">The updated settings</param>
    /// <returns>the updated settings</returns>
    [HttpPut]
    [ProducesResponseType(typeof(GuildSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuildSettingsDto>> UpdateGuildSettings(
        [FromRoute] ulong guildId,
        [FromBody] GuildSettingsUpdateDto guildSettings)
    {
        return Problem("not yet implemented", statusCode: StatusCodes.Status501NotImplemented);
    }
}
