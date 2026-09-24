using System.Collections.Generic;
using System.Threading.Tasks;
using Orange.Api.DTO.Guild;
using Orange.Api.Models;

namespace Orange.Api.Interfaces;

public interface IGuildService
{
    Task<Guild> JoinGuildAsync(GuildJoinDTO guildJoinDto);

    Task<bool> LeaveGuildAsync(ulong id);
    
    Task<List<Guild>> GetGuildsAsync();

    Task<int> GetGuildCountAsync();
}