using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Orange.Bot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _InteractionService;
    private readonly IServiceProvider _services;
    
    public Worker(
        ILogger<Worker> logger, 
        IConfiguration config, 
        InteractionService interactionService, 
        IServiceProvider services)
    {
        _logger = logger;
        _config = config;
        _InteractionService = interactionService;
        _services = services;
        _client = new DiscordSocketClient();
        
        _client.Log += LogAsync;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var token = _config["Discord:Api:Key"]
            ?? throw new InvalidOperationException("Discord API key is not configured.");
        
        
        _client.InteractionCreated += async interaction =>
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _InteractionService.ExecuteCommandAsync(ctx, _services);
        };
        
        _client.Ready += async () =>
        {
            await _client.SetActivityAsync(new Game("Your Mom", ActivityType.Watching));
        };
        
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
