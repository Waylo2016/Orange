using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.Guild;
using Orange.Api.Interfaces;
using Orange.Api.Models;

namespace Orange.Api.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class GuildController(IGuildService guildService) : ControllerBase
{
    /// <summary>
    /// invoked when bot joins a guild
    /// </summary>
    /// <param name="guildJoinDto">the data required to join the guild</param>
    /// <returns>An object representing the joined guild</returns>
    [HttpPost("join")]
    [ProducesResponseType(typeof(GuildJoinDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<GuildJoinDTO>> GuildJoin([FromBody] GuildJoinDTO guildJoinDto)
    {
        Guild guild = await guildService.JoinGuildAsync(guildJoinDto);

        return CreatedAtAction(
            actionName: nameof(GuildJoin),
            routeValues: new
            {
                id = guild.GuildId,
                name = guild.GuildName
            },
            value: new GuildJoinDTO
            {
                GuildId = guild.GuildId,
                GuildName = guild.GuildName
            });
    }

    /// <summary>
    /// invoked when bot leaves a guild
    /// </summary>
    /// <param name="id">the ID of the guild to leave</param>
    /// <returns>removed successfully</returns>
    [HttpDelete("leave/{id:ulong}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GuildLeave([FromRoute] ulong id)
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