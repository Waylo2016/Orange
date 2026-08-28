using Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Orange.Api.utils;
using Orange.Bot.Events;
using xUnit.Tests.Helpers;

using Xunit.Abstractions;

namespace xUnit.Tests.Bot;

public class BotGuildIntegrationTests(ITestOutputHelper output)
{
    private ILogger<GuildEvents> _logger;
    private IConfiguration _configuration;
    private HttpClient _httpClient;
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);


    [Fact]
    public async Task TestBotAndApiHealth()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var app = await new TestAppHostBuilder()
            .WithBot()
            .LogTo(output)
            .WithTimeout(DefaultTimeout)
            .BuildAsync(cancellationToken);

        // Act
        var deadline = DateTime.UtcNow.AddSeconds(60);


        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            "discord-bot", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            "api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var apiClient = app.CreateHttpClient("api");
        var apiResponse = await apiClient.GetAsync("/health", cancellationToken);

        var botClient = app.CreateHttpClient("discord-bot");
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
    // TODO: figure out on how to implement OnGuildJoining and OnGuildLeave, as this feels like a closed-box integration test,
    // where it goes from the bot to the API. 
    // might have to figure out how to instansiate the AppHost, for Aspire DI injection stuffs
    // tested for now on 2 separate servers physically instead of programmatically, but would be nice to have a test for this as well.
}