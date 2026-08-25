using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.Exceptions;
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
    /// <param name="guildIdDto">DTO containing the joined guild id</param>
    /// <returns>A <paramref name="guildIdDto"/> object representing the joined guild</returns>
    [HttpPost("join/{id:long}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Guild>> GuildJoin([FromRoute] ulong id)
    {
        Guild? guild = await guildService.JoinGuildAsync(id);
        
        return CreatedAtAction(
            actionName: nameof(GuildJoin),
            routeValues: new
            {
                guildId = guild.GuildId
            },
            value: guild);
    }

    /// <summary>
    /// invoked when bot leaves a guild
    /// </summary>
    /// <param name="id">The ID of the guild to leave</param>
    /// <returns>A <paramref name="guildLeaveDto"/> object representing the left guild</returns>
    [HttpPost("leave/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IActionResult>> GuildLeave([FromRoute] uint id)
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