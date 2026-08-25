using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Services;

public class GuildService(HttpClient httpClient, ILogger<GuildService> logger) : IGuildService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<GuildService> _logger = logger;

    public async Task RegisterGuildAsync(ulong guildId)
    {
        throw new System.NotImplementedException();
    }

    public async Task UnregisterGuildAsync(ulong guildId)
    {
        throw new System.NotImplementedException();
    }
}