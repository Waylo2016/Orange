using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orange.Bot.Interfaces;
using Orange.Bot.Services;

namespace Orange.Bot;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.AddServiceDefaults();
        builder.AddSeqEndpoint(connectionName: "seq");
        
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddSingleton(new DiscordSocketClient( new DiscordSocketConfig()
            {
                GatewayIntents =   GatewayIntents.Guilds 
                                 | GatewayIntents.DirectMessages 
                                 | GatewayIntents.MessageContent 
                                 | GatewayIntents.GuildMessages
            }));
        
        //singletons for the interaction service
        builder.Services.AddSingleton(sp => 
            new InteractionService(sp.GetRequiredService<DiscordSocketClient>()));
        builder.Services.AddSingleton<IMessageWaiter, MessageWaiter>();
        builder.Services.AddHostedService<SlashCommandRegistrar>();
        
        var host = builder.Build();
        await host.RunAsync();
        
    }
}
