using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orange.Api.DTO.Guild;
using Orange.Api.Interfaces;
using Orange.Api.Models;
using Orange.Api.utils;

namespace Orange.Api.Services;

public class GuildService(ApplicationDbContext _context, ILogger<GuildService> _logger) : IGuildService
{
    public async Task<Guild> JoinGuildAsync(GuildJoinDTO guildJoinDto)
    {
        var guild = new Guild
        {
            GuildId = guildJoinDto.GuildId,
            GuildName = guildJoinDto.GuildName
        };

        _context.Guilds.Add(guild);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Bot invoked join with ID {GuildId}", guild.GuildId);

        return guild;
    }

    public async Task<bool> LeaveGuildAsync(ulong id)
    {
        var guild = await _context.Guilds
            .FirstOrDefaultAsync(g => g.GuildId == id);

        if (guild == null)
        {
            return false;
        }

        _context.Guilds.Remove(guild);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Bot invoked leave with ID {GuildId}", guild.GuildId);
        return true;
    }

    public async Task<int> GetGuildCountAsync()
    {
        return await _context.Guilds.CountAsync();
    }
}