using System.Threading.Tasks;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Orange.Bot.Commands;

public class PingModule(ILogger<PingModule> logger) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Replies with Pong!")]
    public async Task PingAsync()
    {
        logger.LogInformation("[{Source}] Ping command invoked by {User}", "BOT",Context.User.Username);
        await RespondAsync("Pong!, Latency: " + Context.Client.Latency + "ms", ephemeral: true);
    }
}