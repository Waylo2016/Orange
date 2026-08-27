using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Events;

public class GuildEvents(
    ILogger<GuildEvents> logger,
    IConfiguration configuration,
    HttpClient httpClient
    ) : IGuildEvents
{

    public async Task OnGuildJoining(IGuild guild)
    {
        logger.LogInformation($"Joined guild: {guild.Name} ({guild.Id})");

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "/api/v1/Guild/join",
            new
            {
                guildId = guild.Id,
                guildName = guild.Name
            }
        );
        response.EnsureSuccessStatusCode();
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to join guild: {GuildId}", guild.Id);
        }
        
    }
    public async Task OnGuildLeave(IGuild guild)
    {
        logger.LogInformation($"Left guild: {guild.Name} ({guild.Id})");

        HttpResponseMessage response = await httpClient.DeleteAsync(
            $"/api/v1/Guild/leave/{guild.Id}"
        );
        response.EnsureSuccessStatusCode();
    }
}