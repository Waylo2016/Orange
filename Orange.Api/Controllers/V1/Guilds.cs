using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO;
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
    /// <param name="guildJoinDto">DTO containing the joined guild id</param>
    /// <returns>A <paramref name="guildJoinDto"/> object representing the joined guild</returns>
    [HttpPost("join")]
    [ProducesResponseType(typeof(GuildJoinDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<GuildJoinDTO>> GuildJoin([FromBody] GuildJoinDTO guildJoinDto)
    {
        Guild? guild = await guildService.JoinGuildAsync(guildJoinDto);
        
        return CreatedAtAction(
            actionName: nameof(GuildJoin),
            routeValues: new
            {
                guildId = guild.GuildId
            },
            value: guild);
    }
}