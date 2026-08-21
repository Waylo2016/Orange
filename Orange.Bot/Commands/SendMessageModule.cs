using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Orange.Bot.Commands;

public class SendMessageModule(ILogger<SendMessageModule> logger): InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("sendmessage", "sends the user a DM with a message")]
    public async Task SendMessage()
    {
        logger.LogInformation("[{Source}] SendMessage command invoked by user {User}", "BOT", Context.User.Username);
        
        await DeferAsync(ephemeral: true);
        try
        {
            var channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync("Your Mom!");
            await FollowupAsync("Check your dms!", ephemeral: true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Source}] Failed to send DM to {User}", "BOT", Context.User.Username);
            await FollowupAsync("Failed to send DM. Please check your privacy settings.", ephemeral: true);
        }
    }
}