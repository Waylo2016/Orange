
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orange.Api.utils;
using Moq;
using Orange.Api.Services;

namespace xUnit.Tests.Bot;

public class GuildTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;
    private readonly Mock<ILogger<GuildService>> _mockLogger = new();

    private ulong guildId = 123456789012345678; // Example guild ID
    private ulong guildId2 = 2345678901234567890; // Another example guild ID

    [Fact]
    public async Task TestGuildCreation()
    {

        // Arrange
        await using ApplicationDbContext context = new(_dbContextOptions);
        GuildService guildService = new(context, _mockLogger.Object);


        // Act
        await guildService.JoinGuildAsync(guildId);

        // Assert
        var guild = await context.Guilds.FirstOrDefaultAsync(g => g.GuildId == guildId);
        Assert.NotNull(guild);
        Assert.Equal(guildId, guild.GuildId);
    }


    [Fact]
    public async Task TestGuildDeletion()
    {
        // Arrange
        await using ApplicationDbContext context = new(_dbContextOptions);
        GuildService guildService = new(context, _mockLogger.Object);

        // Act
        await guildService.JoinGuildAsync(guildId);
        bool result = await guildService.LeaveGuildAsync(guildId);

        // Assert
        Assert.True(result);
        var guild = await context.Guilds.FirstOrDefaultAsync(g => g.GuildId == guildId);
        Assert.Null(guild);
    }

    [Fact]
    public async Task TestGuildCount()
    {
        // Arrange
        await using ApplicationDbContext context = new(_dbContextOptions);
        GuildService guildService = new(context, _mockLogger.Object);

        // Act
        await guildService.JoinGuildAsync(guildId);
        await guildService.JoinGuildAsync(guildId2);
        int count = await guildService.GetGuildCountAsync();

        // Assert
        Assert.Equal(2, count);
    }
}
