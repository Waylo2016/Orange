using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Orange.Api.DTO.GuildQuestion;
using Orange.Bot.API;
using Orange.Bot.Exceptions;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Commands;

public class CreateQuestionModule(
    ILogger<CreateQuestionModule> logger,
    IMessageWaiter messageWaiter,
    QuestionApiClient questionApiClient) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("createquestion", "Creates a new question for the guild")]
    public async Task CreateQuestion()
    {
        
        logger.LogInformation("[{Source}] CreateQuestion command invoked by user {User}", "Bot", Context.User.Username);
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        Channel channel = Context.Channel;

        var startEmbed = new EmbedProperties
        {
            Title = "Question Creation Process",
            Color = OrangeBotColors.Orange,
            Description =
                "You are about to create a new question for this guild. Please follow the steps below to complete the process.",
            Footer = new EmbedFooterProperties()
            {
                Text = "Question Creation Process"
            }
        };
        
        
        var questionEmbed = new EmbedProperties
        {
            Title = "Step 1: Enter the question",
            Description = "Please type the question you want to create in this channel.",
            Color = OrangeBotColors.Orange,
            Footer = new EmbedFooterProperties()
            {
                Text = "Question Creation Process"
            }
        };

        try
        {
            await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
            {
                Embeds = [startEmbed],
                Flags = MessageFlags.Ephemeral
            });
            Task.Delay(1000).Wait();
            await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
            {
                Embeds = [questionEmbed],
                Flags = MessageFlags.Ephemeral
            });

            Message? questionAnswer =
                await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));

            logger.LogInformation("[{Source}] Received question from user {User}: {Message}", "Bot",
                Context.User.Username, questionAnswer?.Content);

            if (questionAnswer == null)
            {
                await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
                {
                    Content = "No question provided. Please try again.",
                    Flags = MessageFlags.Ephemeral
                });
                return;
            }

            GuildQuestionCreateDto guildQuestionCreateDto = new()
            {
                GuildId = Context.Guild.Id,
                Question = questionAnswer.Content
            };

            GuildQuestionCreateDto? createdQuestion =
                await CreateQuestionWithOrderRetryAsync(guildQuestionCreateDto, channel);

            if (createdQuestion is null)
            {
                return;
            }
            
            
            var resultEmbed = new EmbedProperties()
            {
                Title = "Question Created",
                Description = $"Question: {createdQuestion.Question}\nOrder: {createdQuestion.QuestionOrder}",
                Color = OrangeBotColors.Orange,
                Footer = new EmbedFooterProperties()
                {
                    Text = "Question Creation Process"
                }
            };

            await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
            {
                Embeds = [resultEmbed],
                Flags = MessageFlags.Ephemeral
            });
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "[{Source}] An error occurred while processing the CreateQuestion command for user {User}", "Bot",
                Context.User.Username);
            await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
            {
                Content = "An error occurred while processing your request. Please try again later.",
                Flags = MessageFlags.Ephemeral
            });
        }
    }

    private const int MaxOrderAttempts = 5;

    private async Task<GuildQuestionCreateDto?> CreateQuestionWithOrderRetryAsync(
        GuildQuestionCreateDto guildQuestionCreateDto,
        Channel channel)
    {
        for (int attempt = 1; attempt <= MaxOrderAttempts; attempt++)
        {
            int? questionOrder = await PromptForQuestionOrderAsync(channel, guildQuestionCreateDto.QuestionOrder, attempt);
            if (questionOrder is null)
            {
                // Prompt already sent a Followup explaining the failure.
                return null;
            }

            guildQuestionCreateDto.QuestionOrder = questionOrder.Value;

            try
            {
                return await questionApiClient.CreateQuestionAsync(guildQuestionCreateDto);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogWarning("[{Source}] Conflict when creating question for user {User}: {Message}", "Bot",
                    Context.User.Username, ex.Message);
                await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
                {
                    Content = $"A question with order {guildQuestionCreateDto.QuestionOrder} already exists. Please try again with a different order.",
                    Flags = MessageFlags.Ephemeral
                });
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, "[{Source}] Failed to create question for user {User}", "Bot",
                    Context.User.Username);
                await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
                {
                    Content = "Failed to create question. Please try again later.",
                    Flags = MessageFlags.Ephemeral
                });
                return null;
            }
        }

        await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
        {
            Content = "Too many conflicting orders. Please run the command again.",
            Flags = MessageFlags.Ephemeral
        });
        return null;
    }

    private async Task<int?> PromptForQuestionOrderAsync(Channel channel, int takenNumber, int attempt)
    {
        await SendQuestionOrderPromptAndWaitForResponse(Context.User, channel);
        Message? questionOrderAnswer =
            await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));

        if (questionOrderAnswer == null)
        {
            SendQuestionOrderErrorPromptAndWaitForResponse(Context.User, channel, takenNumber, attempt).Wait();
        }

        if (int.TryParse(questionOrderAnswer.Content, out int questionOrder)) return questionOrder;
        
        await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
        {
            Content = "Invalid question order provided. Please enter a valid number.",
            Flags = MessageFlags.Ephemeral
        });
        return null;

    }

    private async Task<Message?> SendQuestionOrderPromptAndWaitForResponse(User user, Channel channel)
    {
        var questionOrderEmbed =  new EmbedProperties
        {
            Title = "Step 2: Enter the question order",
            Description = "Please type the order number for this question in this channel.",
            Color = OrangeBotColors.Orange,
            Footer = new EmbedFooterProperties()
            {
                Text = "Question Creation Process"
            }
        };

        await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
        {
            Embeds = [questionOrderEmbed],
            Flags = MessageFlags.Ephemeral
        });
        
        return await messageWaiter.WaitForMessageAsync(user.Id, channel.Id, TimeSpan.FromMinutes(2));
    }
    
    private async Task<Message?> SendQuestionOrderErrorPromptAndWaitForResponse(User user, Channel channel, int takenNumber, int attempt)
    {
        var questionOrderEmbed =  new EmbedProperties
        {
            Title = "An error occurred!",
            Description = $"Attempt {attempt} of {MaxOrderAttempts}. The order number {takenNumber} is already taken. Please provide a different order number.",
            Color = OrangeBotColors.Orange,
            Footer = new EmbedFooterProperties()
            {
                Text = "Question Creation Process"
            }
        };

        await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
        {
            Embeds = [questionOrderEmbed],
            Flags = MessageFlags.Ephemeral
        });
        
        return await messageWaiter.WaitForMessageAsync(user.Id, channel.Id, TimeSpan.FromMinutes(2));
    }
}