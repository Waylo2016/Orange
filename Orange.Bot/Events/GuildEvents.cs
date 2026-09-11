using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orange.Bot.DTO.Guild;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Events;

public class GuildEvents(
    ILogger<GuildEvents> logger,
    HttpClient httpClient,
    IConfiguration configuration
    ) : IGuildEvents
{

    public async Task OnGuildJoining(IGuild guild)
    {
        logger.LogInformation("[{Source}] Joined guild: {GuildId}, {GuildName}", "Bot", guild.Id, guild.Name);

        HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/Guilds/");
        request.Headers.Add("X-API-Key", value: configuration["Api:Key"]);
        request.Content = JsonContent.Create(new GuildJoinDTO
        {
            GuildId = guild.Id,
            GuildName = guild.Name
        });

        HttpResponseMessage response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();



        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("[{Source}] Failed to join guild: {GuildId}", "Bot", guild.Id);

        }

    }

    public async Task OnGuildLeave(IGuild guild)
    {
        logger.LogInformation("[{Source}] Left guild: {GuildId}, {GuildName}", "Bot", guild.Id, guild.Name);

        HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/v1/Guilds/{guild.Id}");
        request.Headers.Add("X-API-Key", value: configuration["Api:Key"]);

        HttpResponseMessage response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("[{Source}] Failed to leave guild: {GuildId}", "Bot", guild.Id);
        }
    }
}