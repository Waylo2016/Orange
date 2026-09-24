using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCord.Rest;
using Orange.Bot.DTO.Guild;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Events;

public class GuildEvents(
    ILogger<GuildEvents> logger,
    HttpClient httpClient,
    IConfiguration configuration
    ) : IGuildEvents
{

    public async Task OnGuildJoining(ulong guildId, string guildName)
    {
        logger.LogInformation("[{Source}] Joined guild: {GuildId}, {GuildName}", "Bot", guildId, guildName);

        HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/Guilds/");
        request.Headers.Add("X-API-Key", value: configuration["Api:Key"]);
        request.Content = JsonContent.Create(new GuildJoinDTO
        {
            GuildId = guildId,
            GuildName = guildName
        });

        HttpResponseMessage response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("[{Source}] Failed to join guild: {GuildId}", "Bot", guildId);

        }

    }

    public async Task OnGuildLeave(ulong guildId)
    {
        logger.LogInformation("[{Source}] Left guild: {GuildId}", "Bot", guildId);

        using HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/v1/Guilds/{guildId}");
        request.Headers.Add("X-API-Key", value: configuration["Api:Key"]);

        HttpResponseMessage response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("[{Source}] Failed to leave guild: {GuildId}", "Bot", guildId);
        }
    }
}