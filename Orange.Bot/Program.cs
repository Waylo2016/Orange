using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.DirectMessages | GatewayIntents.MessageContent
            }));
        
        builder.Services.AddSingleton(sp => 
            new InteractionService(sp.GetRequiredService<DiscordSocketClient>()));
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddHostedService<SlashCommandRegistrar>();
        
        var host = builder.Build();
        await host.RunAsync();
        
    }
}
