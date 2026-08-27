using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orange.Bot.Interfaces;

namespace Orange.Bot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _InteractionService;
    private readonly IServiceProvider _services;
    private readonly IGuildEvents _guildEvents;

    public Worker(
        ILogger<Worker> logger,
        IConfiguration config,
        InteractionService interactionService,
        IGuildEvents guildEvents,
        IServiceProvider services,
        DiscordSocketClient client)
    {
        _logger = logger;
        _config = config;
        _InteractionService = interactionService;
        _services = services;
        _client = client;
        _guildEvents = guildEvents;

        _client.Log += LogAsync;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var token = _config["Discord:Api:Key"]
            ?? throw new InvalidOperationException("Discord API key is not configured.");
        
        IGuildEvents guildEvents = _services.GetRequiredService<IGuildEvents>();


        _client.InteractionCreated += async interaction =>
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _InteractionService.ExecuteCommandAsync(ctx, _services);
        };

        _client.Ready += async () =>
        {
            await _client.SetActivityAsync(new Game("Your Mom", ActivityType.Watching));
        };

        
        _client.JoinedGuild += _guildEvents.OnGuildJoining;
        _client.LeftGuild += _guildEvents.OnGuildLeave;

        await _client.LoginAsync(TokenType.Bot, token);


        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task LogAsync(LogMessage message)
    {
        var level = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.None
        };
        _logger.Log(level, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }
}
