
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orange.Api.utils;
using Moq;
using Orange.Api.DTO.Guild;
using Orange.Api.Models;
using Orange.Api.Services;

namespace xUnit.Tests.Bot;

public class GuildTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;
    private readonly Mock<ILogger<GuildService>> _mockLogger = new();

    private const string guildName = "Test Guild"; // Example guild name
    private const string guildName2 = "Test Guild 2"; // Another example guild name

    private const ulong guildId = 123456789012345678; // Example guild ID
    private const ulong GuildId2 = 2345678901234567890; // Another example guild ID


    [Fact]
    public async Task TestGuildCreation()
    {

        // Arrange
        await using ApplicationDbContext context = new(_dbContextOptions);
        GuildService guildService = new(context, _mockLogger.Object);


        // Act
        await guildService.JoinGuildAsync(new GuildJoinDTO()
        {
            GuildId = guildId,
            GuildName = "Test Guild"
        });

        // Assert
        Guild? guild = await context.Guilds.FirstOrDefaultAsync(g => g.GuildId == guildId);

        Assert.NotNull(guild);
        Assert.Equal(guildId, guild.GuildId);
        Assert.NotNull(guild.GuildName);
        Assert.Equal(guildName, guild.GuildName);
    }


    [Fact]
    public async Task TestGuildDeletion()
    {
        // Arrange
        await using ApplicationDbContext context = new(_dbContextOptions);
        GuildService guildService = new(context, _mockLogger.Object);

        // Act
        await guildService.JoinGuildAsync(new GuildJoinDTO()
        {
            GuildId = guildId,
            GuildName = guildName
        });
        bool result = await guildService.LeaveGuildAsync(new GuildLeaveDTO()
        {
            GuildId = guildId
        });

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
        await guildService.JoinGuildAsync(new GuildJoinDTO()
        {
            GuildId = guildId,
            GuildName = guildName
        });
        await guildService.JoinGuildAsync(new GuildJoinDTO()
        {
            GuildId = GuildId2,
            GuildName = guildName2
        });
        int count = await guildService.GetGuildCountAsync();

        // Assert
        Assert.Equal(2, count);
    }
}
