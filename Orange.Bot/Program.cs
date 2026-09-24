using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;

using Orange.Bot.API;
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

        builder.Services.AddDiscordGateway(options =>
            {
                options.Intents = GatewayIntents.Guilds
                                  | GatewayIntents.DirectMessages
                                  | GatewayIntents.MessageContent
                                  | GatewayIntents.GuildMessages;
                options.Presence = new PresenceProperties(UserStatusType.Online)
                {
                    Activities = [new UserActivityProperties("Your Mom", UserActivityType.Watching)]
                };
                options.Token = new string(builder.Configuration["Discord:Api:Key"]!
                                           ?? throw new InvalidOperationException("Discord bot token is not configured."));
            })
            .AddGatewayHandlers(typeof(Program).Assembly);
        
        builder.Services.AddSingleton<IMessageWaiter, MessageWaiter>();

        // Health checks
        builder.Services.AddHealthChecks()
            .AddCheck<DiscordConnectionHealthCheck>(
                "discord",
                failureStatus: HealthStatus.Unhealthy);

        builder.Services.AddHttpClient<IGuildEvents, GuildEvents>(client =>
        {
            client.BaseAddress = new Uri("https+http://orange-api")
                                 ?? throw new InvalidOperationException("API base URL is not configured.");
        });
        builder.Services.AddHttpClient<QuestionApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://orange-api")
                                 ?? throw new InvalidOperationException("API base URL is not configured.");
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();   // from Aspire, maps /health and /alive

        await app.RunAsync();

    }
}
