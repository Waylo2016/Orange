using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
    private readonly string? _baseurl = configuration["Api:BaseUrl"];

    public async Task OnGuildJoining(SocketGuild guild)
    {
        logger.LogInformation($"Joined guild: {guild.Name} ({guild.Id})");

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            $"{_baseurl}/api/v1/Guild/join",
            new
            {
                guildId = guild.Id,
                guildName = guild.Name
            }
        );
        response.EnsureSuccessStatusCode();

    }
    public async Task OnGuildLeave(SocketGuild guild)
    {
        logger.LogInformation($"Left guild: {guild.Name} ({guild.Id})");
        
        HttpResponseMessage response = await httpClient.DeleteAsync(
            $"{_baseurl}/api/v1/Guild/leave/{guild.Id}"
        );
        response.EnsureSuccessStatusCode();
    }
}