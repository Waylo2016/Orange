using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.Interfaces;
using Orange.Api.Models;

namespace Orange.Api.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
public class GuildController(IGuildService guildService) : ControllerBase
{
    /// <summary>
    /// invoked when bot joins a guild
    /// </summary>
    /// <param name="id">The ID of the guild to join</param>
    /// <returns>An object representing the joined guild</returns>
    [HttpPost("join/{id:ulong}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Guild>> GuildJoin([FromRoute] ulong id)
    {
        Guild guild = await guildService.JoinGuildAsync(id);

        return CreatedAtAction(
            actionName: nameof(GuildJoin),
            routeValues: new
            {
                id = guild.GuildId
            },
            value: guild);
    }

    /// <summary>
    /// invoked when bot leaves a guild
    /// </summary>
    /// <param name="id">The ID of the guild to leave</param>
    /// <returns>removed successfully</returns>
    [HttpPost("leave/{id:ulong}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IActionResult>> GuildLeave([FromRoute] ulong id)
    {

        bool result = await guildService.LeaveGuildAsync(id);
        return result ? NoContent() : Problem("An unexpected error occurred while trying to leave the guild.");

    }

    /// <summary>
    /// Get the total number of guilds the bot is in
    /// </summary>
    /// <returns>The total number of guilds the bot is in</returns>
    [HttpGet("count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetGuildCount()
    {
        int count = await guildService.GetGuildCountAsync();
        return Ok(count);
    }

}