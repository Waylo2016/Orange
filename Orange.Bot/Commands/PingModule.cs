using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Orange.Bot.Commands;

public class PingModule(ILogger<PingModule> logger) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("ping", "Replies with Pong!")]
    public async Task PingAsync()
    {
        logger.LogInformation("[{Source}] Ping command invoked by {User}", "BOT", Context.User.Username);
        await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
        {
            Content = "Pong!, Latency: " + Context.Client.Latency + "ms"
        });
    }
}