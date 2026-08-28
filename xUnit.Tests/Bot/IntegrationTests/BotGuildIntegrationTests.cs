using System.Net.Http.Json;
using Discord;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Orange.Bot.Events;
using Xunit.Abstractions;
using xUnit.Tests.Helpers;

namespace xUnit.Tests.Bot.IntegrationTests;

public class BotGuildIntegrationTests(ITestOutputHelper output)
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);


    [Fact]
    public async Task TestBotAndApiHealth()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        string discordBot = "discord-bot";
        string orangeApi = "orange-api";

        var app = await new TestAppHostBuilder()
            .WithBot()
            .LogTo(output)
            .WithTimeout(DefaultTimeout)
            .BuildAsync(cancellationToken);

        // Act
        var deadline = DateTime.UtcNow.AddSeconds(60);


        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            discordBot, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            orangeApi, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var apiClient = app.CreateHttpClient(orangeApi);
        var apiResponse = await apiClient.GetAsync("/health", cancellationToken);

        var botClient = app.CreateHttpClient(discordBot);
        var botResponse = await botClient.GetAsync("/health", cancellationToken);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var lastBotResponse = await botClient.GetAsync("/health", cancellationToken);
                var lastBotBody = await lastBotResponse.Content.ReadAsStringAsync(cancellationToken);

                if ((lastBotResponse.StatusCode == HttpStatusCode.OK && lastBotBody == "Healthy"))
                {
                    return; // success
                }
            }
            catch (HttpRequestException)
            {
                // bot HTTP endpoint nog niet open, retry
            }

            await Task.Delay(500, cancellationToken);
        }

        // Assert
        Assert.Fail($"Bot did not become healthy within the timeout period of {DefaultTimeout.TotalSeconds} seconds. "
            + $"Last API response: {apiResponse.StatusCode}, Last Bot response: {botResponse.StatusCode}");
    }

    [Fact]
    public async Task TestBotGuildJoin()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        const string discordBot = "discord-bot";
        const string orangeApi = "orange-api";
        const string guildName = "Test Guild";
        const ulong guildId = 123456789012345678; // Example

        // Discord's IGuild is the boundary we don't control, so it's fine to fake it.
        // Everything downstream of it (GuildEvents -> HTTP -> API) should be real.
        var substituteGuild = Substitute.For<IGuild>();
        substituteGuild.Id.Returns(guildId);
        substituteGuild.Name.Returns(guildName);

        var app = await new TestAppHostBuilder()
            .WithBot()
            .LogTo(output)
            .WithTimeout(DefaultTimeout)
            .BuildAsync(cancellationToken);

        // Act
        await app.ResourceNotifications.WaitForResourceHealthyAsync(
                discordBot, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync(
                orangeApi, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var apiClient = app.CreateHttpClient(orangeApi);

        int countBefore = await apiClient.GetFromJsonAsync<int>("/api/v1/Guild/count", cancellationToken);

        var guildEvents = new GuildEvents(NullLogger<GuildEvents>.Instance, apiClient);
        await guildEvents.OnGuildJoining(substituteGuild);

        int countAfter = await apiClient.GetFromJsonAsync<int>("/api/v1/Guild/count", cancellationToken);

        // Assert
        Assert.Equal(countBefore + 1, countAfter);
    }

    [Fact]
    public async Task TestBotGuildLeave()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        const string discordBot = "discord-bot";
        const string orangeApi = "orange-api";
        const string guildName = "Test Guild";
        const ulong guildId = 123456789012345678; // Example

        // Discord's IGuild is the boundary we don't control, so it's fine to fake it.
        // Everything downstream of it (GuildEvents -> HTTP -> API) should be real.
        var substituteGuild = Substitute.For<IGuild>();
        substituteGuild.Id.Returns(guildId);
        substituteGuild.Name.Returns(guildName);

        var app = await new TestAppHostBuilder()
            .WithBot()
            .LogTo(output)
            .WithTimeout(DefaultTimeout)
            .BuildAsync(cancellationToken);

        // Act
        await app.ResourceNotifications.WaitForResourceHealthyAsync(
                discordBot, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync(
                orangeApi, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var apiClient = app.CreateHttpClient(orangeApi);
        var guildEvents = new GuildEvents(NullLogger<GuildEvents>.Instance, apiClient);

        // first we have to join the guild to ensure it exists in the API before we can leave it
        await guildEvents.OnGuildJoining(substituteGuild);
        int countBefore = await apiClient.GetFromJsonAsync<int>("/api/v1/Guild/count", cancellationToken);

        // then we can leave the guild
        await guildEvents.OnGuildLeave(substituteGuild);

        int countAfter = await apiClient.GetFromJsonAsync<int>("/api/v1/Guild/count", cancellationToken);

        // Assert
        Assert.Equal(countBefore - 1, countAfter);
    }
}