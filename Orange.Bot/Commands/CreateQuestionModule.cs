using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Orange.Api.DTO.GuildQuestion;
using Orange.Bot.API;
using Orange.Bot.Exceptions;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Commands;

public class CreateQuestionModule(
    ILogger<CreateQuestionModule> logger,
    IMessageWaiter messageWaiter,
    QuestionApiClient questionApiClient) : InteractionModuleBase<SocketInteractionContext>
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

        try
        {
            await FollowupAsync(embed: startEmbed.Build(), ephemeral: true);
            Task.Delay(1000).Wait();
            await FollowupAsync(embed: questionEmbed.Build(), ephemeral: true);

            SocketMessage? questionAnswer =
                await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));

            logger.LogInformation("[{Source}] Received question from user {User}: {Message}", "Bot",
                Context.User.Username, questionAnswer?.Content);

            if (questionAnswer == null)
            {
                await FollowupAsync("No question provided. Please try again.", ephemeral: true);
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
                // A terminal failure already sent its own Followup message.
                return;
            }

            var resultEmbed = new EmbedBuilder()
                .AddField("Question Created",
                    $"Question: {createdQuestion.Question}\nOrder: {createdQuestion.QuestionOrder}")
                .WithFooter(footer => footer.WithText("Question Creation Process"))
                .WithColor(OrangeBotColors.Orange);

            await FollowupAsync(embed: resultEmbed.Build(), ephemeral: true);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "[{Source}] An error occurred while processing the CreateQuestion command for user {User}", "Bot",
                Context.User.Username);
            await FollowupAsync("An error occurred while processing your request. Please try again later.",
                ephemeral: true);
        }
    }

    private const int MaxOrderAttempts = 5;

    private async Task<GuildQuestionCreateDto?> CreateQuestionWithOrderRetryAsync(
        GuildQuestionCreateDto guildQuestionCreateDto,
        IChannel channel)
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
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                logger.LogWarning("[{Source}] Conflict when creating question for user {User}: {Message}", "Bot",
                    Context.User.Username, ex.Message);
                await FollowupAsync(
                    $"A question with order {guildQuestionCreateDto.QuestionOrder} already exists. Please try again with a different order.",
                    ephemeral: true);
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, "[{Source}] Failed to create question for user {User}", "Bot",
                    Context.User.Username);
                await FollowupAsync("Failed to create question. Please try again later.", ephemeral: true);
                return null;
            }
        }

        await FollowupAsync("Too many conflicting orders. Please run the command again.", ephemeral: true);
        return null;
    }

    private async Task<int?> PromptForQuestionOrderAsync(IChannel channel, int takenNumber, int attempt)
    {
        await SendQuestionOrderPromptAndWaitForResponse(Context.User, channel);
        SocketMessage? questionOrderAnswer =
            await messageWaiter.WaitForMessageAsync(Context.User.Id, channel.Id, TimeSpan.FromMinutes(1));

        if (questionOrderAnswer == null)
        {
            SendQuestionOrderErrorPromptAndWaitForResponse(Context.User, channel, takenNumber, attempt).Wait();
        }

        if (!int.TryParse(questionOrderAnswer.Content, out int questionOrder))
        {
            await FollowupAsync("Invalid question order provided. Please enter a valid number.", ephemeral: true);
            return null;
        }

        return questionOrder;
    }

    private async Task<IUserMessage?> SendQuestionOrderPromptAndWaitForResponse(SocketUser user, IChannel channel)
    {
        var questionOrderEmbed = new EmbedBuilder()
            .AddField("Step 2: Enter the question order",
                "Please type the order number for this question in this channel.")
            .WithFooter(footer => footer.WithText("Question Creation Process"))
            .WithColor(OrangeBotColors.Orange);

        return await FollowupAsync(embed: questionOrderEmbed.Build(), ephemeral: true);
    }
    
    private async Task<IUserMessage?> SendQuestionOrderErrorPromptAndWaitForResponse(SocketUser user, IChannel channel, int takenNumber, int attempt)
    {
        var questionOrderEmbed = new EmbedBuilder()
            .AddField("An error occurred!",
                "Please provide a valid question order number for this question in this channel.")
            .AddField("Attempt",
                $"Attempt {attempt} of {MaxOrderAttempts}. The order number {takenNumber} is already taken. Please provide a different order number.")
            .WithFooter(footer => footer.WithText("Question Creation Process"))
            .WithColor(OrangeBotColors.Orange);

        return await FollowupAsync(embed: questionOrderEmbed.Build(), ephemeral: true);
    }
}