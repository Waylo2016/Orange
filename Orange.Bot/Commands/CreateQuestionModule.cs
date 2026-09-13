using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Orange.Api.DTO.GuildQuestion;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Commands;

public class CreateQuestionModule(ILogger<CreateQuestionModule> logger, IMessageWaiter messageWaiter ) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("createquestion", "Creates a new question for the guild")]
    public async Task CreateQuestion()
    {
        logger.LogInformation("[{Source}] CreateQuestion command invoked by user {User}", "Bot", Context.User.Username);
        await DeferAsync(ephemeral: true);
        IChannel? channel = Context.Channel;

        var startEmbed = new EmbedBuilder()
            .AddField("Question Creator",
                "This command will guide you through the process of creating a new question for your Discord server. Please follow the prompts and provide the necessary information.")
            .WithFooter(footer => footer.WithText("Question Creation Process"))
            .WithColor(OrangeBotColors.Orange);

        var questionEmbed = new EmbedBuilder()
            .AddField("Step 1: Enter the question", "Please type the question you want to create in this channel.")
            .WithFooter(footer => footer.WithText("Question Creation Process"))
            .WithColor(OrangeBotColors.Orange);

        var questionOrderEmbed = new EmbedBuilder()
            .AddField("Step 2: Enter the question order",
                "Please type the order number for this question in this channel.")
            .WithFooter(footer => footer.WithText("Question Creation Process"))
            .WithColor(OrangeBotColors.Orange);
        
        try
        {
            
            try
            {
                await FollowupAsync(embed: startEmbed.Build(), ephemeral: true);
                Task.Delay(1000).Wait(); // Wait for 1 second before sending the next message
                await FollowupAsync(embed: questionEmbed.Build(), ephemeral: true);
                
                SocketMessage? questionAnswer = await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));
                
                logger.LogInformation("[{Source}] Received question from user {User}: {Message}", "Bot", Context.User.Username, questionAnswer?.Content);
                
                if (questionAnswer == null)
                {
                    await FollowupAsync("No question provided. Please try again.", ephemeral: true);
                    return;
                }

                await FollowupAsync(embed: questionOrderEmbed.Build(), ephemeral: true);
                SocketMessage? questionOrderAnswer = await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));
                
                if (questionOrderAnswer == null)
                {
                    await FollowupAsync("No question order provided. Please try again.", ephemeral: true);
                    return;
                }
                
                if (!int.TryParse(questionOrderAnswer.Content, out int questionOrder))
                {
                    await FollowupAsync("Invalid question order provided. Please enter a valid integer.", ephemeral: true);
                    return;
                }
                
                var guildQuestionCreateDto = new GuildQuestionCreateDto
                {
                    GuildId = Context.Guild.Id,
                    Question = questionAnswer.Content,
                    QuestionOrder = questionOrder
                };

                var resultEmbed = new EmbedBuilder()
                    .AddField("Question Created",
                        $"Question: {guildQuestionCreateDto.Question}\nOrder: {guildQuestionCreateDto.QuestionOrder}")
                    .WithFooter(footer => footer.WithText("Question Creation Process"))
                    .WithColor(OrangeBotColors.Orange);
                
                await FollowupAsync(embed: resultEmbed.Build(), ephemeral: true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "[{Source}] Failed to create question for user {User}", "Bot", Context.User.Username);
                await FollowupAsync("Failed to create question. Please try again later.", ephemeral: true);
                return;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Source}] An error occurred while processing the CreateQuestion command for user {User}", "Bot", Context.User.Username);
            await FollowupAsync("An error occurred while processing your request. Please try again later.", ephemeral: true);
        }
        
    }
}