using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.Guild;
using Orange.Api.Interfaces;
using Orange.Api.Models;

namespace Orange.Api.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Tags("Guilds")]
[Produces("application/json")]
public class GuildsController(IGuildService guildService) : ControllerBase
{
    /// <summary>
    /// invoked when the bot joins a guild
    /// </summary>
    /// <remarks>Auth: bot only</remarks>
    /// <param name="guildJoinDto">the data required to join the guild</param>
    /// <returns>An object representing the joined guild</returns>

    [HttpPost]
    [Authorize(AuthenticationSchemes = "ApiKey", Policy = "BotOnly")]
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
    /// invoked when the bot leaves a guild
    /// </summary>
    /// <remarks>Auth: bot only</remarks>
    /// <param name="guildId">the ID of the guild to leave</param>
    /// <returns>removed successfully</returns>
    [HttpDelete("{guildId:ulong}")]
    [Authorize(AuthenticationSchemes = "ApiKey", Policy = "BotOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GuildLeave([FromRoute] ulong guildId)
    {
        bool result = await guildService.LeaveGuildAsync(guildId);
        return result ? NoContent() : Problem("An unexpected error occurred while trying to leave the guild.");
    }

    /// <summary>
    /// Get the total number of guilds the bot is in
    /// </summary>
    /// <remarks>Auth: bot only</remarks>
    /// <returns>The total number of guilds the bot is in</returns>
    [HttpGet("count")]
    [Authorize(AuthenticationSchemes = "ApiKey", Policy = "BotOnly")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetGuildCount()
    {
        int count = await guildService.GetGuildCountAsync();
        return Ok(count);
    }
    
    //TODO: maybe change it so the leave server can be called by people logged into the web dashboard
}
