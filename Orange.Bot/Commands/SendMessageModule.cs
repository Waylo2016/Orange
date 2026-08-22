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
            IDMChannel? channel = await Context.User.CreateDMChannelAsync();
            await channel.SendMessageAsync("Your Mom!");
            await FollowupAsync("Check your dms!", ephemeral: true);
            
            SocketMessage? userAnswer = await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromSeconds(20));
            logger.LogInformation("[{Source}] Received message from user {User}: {Message}", "Bot", Context.User.Username, userAnswer?.Content);
            
            
            ISocketMessageChannel? invokationChannel = Context.Channel;
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
            logger.LogError(e, "[{Source}] Failed to send DM to {User}", "Bot", Context.User.Username);
            await FollowupAsync("Failed to send DM. Please check your privacy settings.", ephemeral: true);
        }
    }
}