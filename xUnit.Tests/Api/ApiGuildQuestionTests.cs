using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Orange.Api.Controllers.V1;
using Orange.Api.DTO.Guild;
using Orange.Api.DTO.GuildQuestion;
using Orange.Api.Models;
using Orange.Api.Services;
using Orange.Api.utils;
using xUnit.Tests.Helpers;

namespace xUnit.Tests.Api;

public class ApiGuildQuestionTests(SqliteFixture sqliteFixture, ITestOutputHelper output)
    : IClassFixture<SqliteFixture>, IAsyncLifetime
{


    private readonly ApplicationDbContext _dbContext = sqliteFixture.CreateDbContext();

    private static readonly ILogger<GuildQuestionService> _nsubLogger = Substitute.For<ILogger<GuildQuestionService>>();

    private GuildQuestionService _guildQuestionService = null!;

    // Runs before each [Fact]
    public async ValueTask InitializeAsync()
    {
        await sqliteFixture.ResetAsync();
        _guildQuestionService = new GuildQuestionService(_dbContext, _nsubLogger);
    }

    public ValueTask DisposeAsync() => _dbContext.DisposeAsync();

    // example guilds
    private const string guildName = "Test Guild";
    private const string guildName2 = "Test Guild 2";

    private const ulong guildId = 123456789012345678;
    private const ulong GuildId2 = 2345678901234567890;

    // example questions
    private const string question1 = "What is your favorite color?";
    private const string question2 = "What is your favorite food?";
    private const string question3 = "What is your favorite movie?";

    private const int questionOrder1 = 1;
    private const int questionOrder2 = 2;
    private const int questionOrder3 = 3;

    [Fact]
    public async Task TestQuestionCreation()
    {
        // Arrange
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = guildId,
            GuildName = guildName
        }, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var createdQuestion = await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = guildId,
            Question = question1,
            QuestionOrder = questionOrder1
        });

        // Assert
        Assert.NotNull(createdQuestion);
        Assert.Equal(guildId, createdQuestion.GuildId);
        Assert.Equal(question1, createdQuestion.Question);
        Assert.Equal(questionOrder1, createdQuestion.QuestionOrder);

    }

    [Fact]
    public async Task EnsureQuestionDoesntCrossOverToAnotherGuild()
    {
        // Arrange
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = guildId,
            GuildName = guildName
        }, TestContext.Current.CancellationToken);
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = GuildId2,
            GuildName = guildName2
        }, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var createdQuestion1 = await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = guildId,
            Question = question1,
            QuestionOrder = questionOrder1
        });

        var createdQuestion2 = await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = GuildId2,
            Question = question2,
            QuestionOrder = questionOrder2
        });

        // Act

        var questionsForGuild1 = await _guildQuestionService.GetGuildQuestionsPerGuildAsync(new GuildIdDTO
        {
            GuildId = guildId
        });

        var questionsForGuild2 = await _guildQuestionService.GetGuildQuestionsPerGuildAsync(new GuildIdDTO
        {
            GuildId = GuildId2
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(createdQuestion1);
            Assert.NotNull(createdQuestion2);
            var item1 = Assert.Single(questionsForGuild1);
            var item2 = Assert.Single(questionsForGuild2);
            Assert.NotEqual(item1.Id, item2.Id);
        });
    }

    [Fact]
    public async Task TestQuestionRemoval()
    {
        // Arrange
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = guildId,
            GuildName = guildName
        }, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = guildId,
            Question = question1,
            QuestionOrder = questionOrder1
        });
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var deletedQuestion = await _guildQuestionService.DeleteGuildQuestionAsync(new GuildQuestionOrderDeleteDto
        {
            GuildId = guildId,
            QuestionOrder = questionOrder1
        });

        // Assert
        Assert.True(deletedQuestion);
    }

    [Fact]
    public async Task TestQuestionRetrievalByQuestionOrder()
    {
        // Arrange
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = guildId,
            GuildName = guildName
        }, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = guildId,
            Question = question1,
            QuestionOrder = questionOrder1
        });

        var retrievedQuestion = await _guildQuestionService.GetGuildQuestionByIdAsync(new GuildQuestionOrderDeleteDto
        {
            GuildId = guildId,
            QuestionOrder = questionOrder1
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(retrievedQuestion);
            Assert.Equal(guildId, retrievedQuestion.GuildId);
            Assert.Equal(question1, retrievedQuestion.Question);
            Assert.Equal(questionOrder1, retrievedQuestion.QuestionOrder);
        });
    }

    [Fact]
    public async Task TestQuestionListGet_ReturnsListOfQuestions()
    {
        // Arrange
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = guildId,
            GuildName = guildName
        }, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = guildId,
            Question = question1,
            QuestionOrder = questionOrder1
        });

        await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = guildId,
            Question = question2,
            QuestionOrder = questionOrder2
        });

        // Act
        var questionsList = await _guildQuestionService.GetGuildQuestionsPerGuildAsync(new GuildIdDTO
        {
            GuildId = guildId
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(questionsList);
            Assert.Equal(2, questionsList.Count);
            Assert.Equal(question1, questionsList[0].Question);
            Assert.Equal(questionOrder1, questionsList[0].QuestionOrder);
            Assert.Equal(question2, questionsList[1].Question);
            Assert.Equal(questionOrder2, questionsList[1].QuestionOrder);
        });
    }

    [Fact]
    public async Task TestQuestionListGet_ReturnsEmptyListForNoQuestions()
    {
        // Arrange
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = guildId,
            GuildName = guildName
        }, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var questionsList = await _guildQuestionService.GetGuildQuestionsPerGuildAsync(new GuildIdDTO
        {
            GuildId = guildId
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(questionsList);
            Assert.Empty(questionsList);
        });
    }

    [Fact]
    public async Task TestQuestionListGet_ReturnsNotFoundForNonExistentGuild()
    {
        // Arrange 
        ulong nonExistentGuildId = 999999999999999999;

        // Act
        var questionsList = await _guildQuestionService.GetGuildQuestionsPerGuildAsync(new GuildIdDTO
        {
            GuildId = nonExistentGuildId,
        });

        // Assert
        Assert.Empty(questionsList);
    }

    [Fact]
    public async Task TestQuestionUpdate()
    {
        // Arrange
        await _dbContext.Guilds.AddAsync(new Guild
        {
            GuildId = guildId,
            GuildName = guildName
        }, TestContext.Current.CancellationToken);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _guildQuestionService.CreateGuildQuestionAsync(new GuildQuestionCreateDto
        {
            GuildId = guildId,
            Question = question1,
            QuestionOrder = questionOrder1
        });
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var updatedQuestion = await _guildQuestionService.UpdateGuildQuestionAsync(new GuildQuestionUpdateDto
        {
            GuildId = guildId,
            OldQuestionOrder = questionOrder1,
            NewQuestion = question2,
            NewQuestionOrder = questionOrder1
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(updatedQuestion);
            Assert.Equal(guildId, updatedQuestion.GuildId);
            Assert.Equal(question2, updatedQuestion.Question);
            Assert.Equal(questionOrder1, updatedQuestion.QuestionOrder);
        });
    }

}