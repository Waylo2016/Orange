using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Orange.Bot;

public class SlashCommandRegistrar : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;
    private readonly ILogger<SlashCommandRegistrar> _logger;
    private readonly TaskCompletionSource _readyTcs = new();
    
    public SlashCommandRegistrar(
        DiscordSocketClient client,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IConfiguration config,
        ILogger<SlashCommandRegistrar> logger)
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _config = config;
        _logger = logger;

        _client.Ready += OnReady;
    }
    
    private Task OnReady()
    {
        _readyTcs.TrySetResult();
        return Task.CompletedTask;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        
        await _readyTcs.Task.WaitAsync(cancellationToken);

        var devGuildId = _config.GetValue<ulong?>("Discord:DevGuildId");
        if (devGuildId.HasValue)
        {
            await _interactionService.RegisterCommandsToGuildAsync(devGuildId.Value);
            _logger.LogInformation("[{Source}] | Slash commands registered to guild {GuildId}", "SlashCommandRegistrar", devGuildId.Value);
        }
        else
        {
            await _interactionService.RegisterCommandsGloballyAsync();
            _logger.LogInformation("[{Source}] | Slash commands registered globally", "SlashCommandRegistrar");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}