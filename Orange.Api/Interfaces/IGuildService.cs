using System.Threading.Tasks;
using Orange.Api.DTO;
using Orange.Api.Models;

namespace Orange.Api.Interfaces;

public interface IGuildService
{
    Task<Guild> JoinGuildAsync(GuildJoinDTO guildJoinDto);
}