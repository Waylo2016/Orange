using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Commands;

public class SendMessageModule(ILogger<SendMessageModule> logger, IMessageWaiter messageWaiter) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("sendmessage", "sends the user a DM with a message")]
    public async Task SendMessage()
    {
        logger.LogInformation("[{Source}] SendMessage command invoked by user {User}", "Bot", Context.User.Username);

        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        try
        {
            DMChannel? channel;

            try
            {
                channel = await Context.User.GetDMChannelAsync();
                await channel.SendMessageAsync("Your Mom");

            }
            catch (Exception e)
            {
                logger.LogError(e, "[{Source}] Failed to send DM to {User}", "Bot", Context.User.Username);
                await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
                {
                    Content = "Failed to send DM. Please check your privacy settings.",
                    Flags = MessageFlags.Ephemeral
                });
                return;
            }

            await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
            {
                Content = "Check your DMs!",
                Flags = MessageFlags.Ephemeral
            });

            Message? userAnswer = await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));
            logger.LogInformation("[{Source}] Received message from user {User}: {Message}", "Bot", Context.User.Username, userAnswer?.Content);
            

            if (userAnswer != null)
            {
                await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
                {
                    Content = $"{Context.User.Username} said: {userAnswer.Content}",
                    Flags = MessageFlags.Ephemeral
                });
            }
            else
            {
                await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
                {
                    Content = "You did not respond in time.",
                    Flags = MessageFlags.Ephemeral
                });
            }

        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Source}] An unexpected error occurred while processing the SendMessage command for user {User}", "Bot", Context.User.Username);
            await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
            {
                Content = "An unexpected error occurred. Please try again later.",
                Flags = MessageFlags.Ephemeral
            });
        }
    }
}