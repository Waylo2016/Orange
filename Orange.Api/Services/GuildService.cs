using System.Net.Mime;
using System.Threading.Tasks;
using Orange.Api.DTO;
using Orange.Api.Interfaces;
using Orange.Api.Models;
using Orange.Api.utils;

namespace Orange.Api.Services;

public class GuildService(ApplicationDbContext _context) : IGuildService
{
    public async Task<Guild> JoinGuildAsync(GuildJoinDTO guildJoinDto)
    {
        var guild = new Guild
        {
            GuildId = guildJoinDto.GuildId
        };

        _context.Guilds.Add(guild);
        await _context.SaveChangesAsync();

        return guild;
    }
}