using System.Threading.Tasks;
using Orange.Api.DTO.Guild;
using Orange.Api.Models;

namespace Orange.Api.Interfaces;

public interface IGuildService
{
    Task<Guild> JoinGuildAsync(GuildJoinDTO guildJoinDto);

    Task<bool> LeaveGuildAsync(GuildLeaveDTO guildLeaveDto);

    Task<int> GetGuildCountAsync();
}