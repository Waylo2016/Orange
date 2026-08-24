using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Commands;

public class SendMessageModule(ILogger<SendMessageModule> logger, IMessageWaiter messageWaiter): InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("sendmessage", "sends the user a DM with a message")]
    public async Task SendMessage()
    {
        
        logger.LogInformation("[{Source}] SendMessage command invoked by user {User}", "Bot", Context.User.Username);
        
        await DeferAsync(ephemeral: true);
        try
        {
            IDMChannel? channel = null;
            
            try
            {
                channel = await Context.User.CreateDMChannelAsync();
                await channel.SendMessageAsync("Your Mom!");
                
            }
            catch (Exception e)
            {
                logger.LogError(e, "[{Source}] Failed to send DM to {User}", "Bot", Context.User.Username);
                await FollowupAsync("Failed to send DM. Please check your privacy settings.", ephemeral: true);
                return;
            }
            
            await FollowupAsync("Check your dms!", ephemeral: true);
            
            SocketMessage? userAnswer = await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));
            logger.LogInformation("[{Source}] Received message from user {User}: {Message}", "Bot", Context.User.Username, userAnswer?.Content);
            
            ISocketMessageChannel? invokationChannel = Context.Channel;
            
            if (invokationChannel == null)
            {
                logger.LogWarning("[{Source}] Invocation channel is null for user {User}", "Bot", Context.User.Username);
                await FollowupAsync("Couldn't find the channel to post your reply in.", ephemeral: true);
                return;
            }
            if (userAnswer != null)
            {
                await invokationChannel.SendMessageAsync($"User {Context.User.Username} responded with: {userAnswer.Content}");
            }
            else
            {
                await invokationChannel.SendMessageAsync($"User {Context.User.Username} did not respond within the timeout period.");
            }
            
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "[{Source}] An unexpected error occurred while processing the SendMessage command for user {User}", "Bot", Context.User.Username);
            await FollowupAsync("An unexpected error occurred. Please try again later.", ephemeral: true);
        }
    }
}