using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Orange.Bot.Events;
using Orange.Bot.HealthChecks;
using Orange.Bot.Interfaces;
using Orange.Bot.Services;

namespace Orange.Bot;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add logging and configuration
        builder.AddServiceDefaults();
        builder.AddSeqEndpoint(connectionName: "seq");

        
        // Add services to the container.

        builder.Services.AddOpenTelemetry();
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
                             | GatewayIntents.DirectMessages
                             | GatewayIntents.MessageContent
        }));

        builder.Services.AddSingleton(sp =>
            new InteractionService(sp.GetRequiredService<DiscordSocketClient>()));
        builder.Services.AddSingleton<IMessageWaiter, MessageWaiter>();
        builder.Services.AddHostedService<SlashCommandRegistrar>();

        // Health checks
        builder.Services.AddHealthChecks()
            .AddCheck<DiscordConnectionHealthCheck>(
                "discord",
                failureStatus: HealthStatus.Unhealthy);

        builder.Services.AddHttpClient<IGuildEvents, GuildEvents>(client =>
        {
            client.BaseAddress = new Uri("https+http://api")
                                 ?? throw new InvalidOperationException("API base URL is not configured.");
        });
        
        var app = builder.Build();

        app.MapDefaultEndpoints();   // uit Orange.ServiceDefaults, mapt /health en /alive

        await app.RunAsync();

    }
}
