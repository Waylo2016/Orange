using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Orange.Api.utils;
using Orange.Api.DTO.Guild;
using Orange.Api.Models;
using Orange.Api.Services;

namespace xUnit.Tests.Api;

public class ApiGuildTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    private readonly ILogger<GuildService> _nsubLogger = Substitute.For<ILogger<GuildService>>();

    private const string guildName = "Test Guild"; // Example guild name
    private const string guildName2 = "Test Guild 2"; // Another example guild name

    private const ulong guildId = 123456789012345678; // Example guild ID
    private const ulong GuildId2 = 2345678901234567890; // Another example guild ID


    [Fact]
    public async Task TestGuildCreation()
    {

        // Arrange
        await using ApplicationDbContext context = new(_dbContextOptions);
        GuildService guildService = new(context, _nsubLogger);


        // Act
        await guildService.JoinGuildAsync(new GuildJoinDTO()
        {
            GuildId = guildId,
            GuildName = "Test Guild"
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        Guild? guild = await context.Guilds.FirstOrDefaultAsync(g => g.GuildId == guildId, cancellationToken: TestContext.Current.CancellationToken);

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
        GuildService guildService = new(context, _nsubLogger);

        // Act
        await guildService.JoinGuildAsync(new GuildJoinDTO()
        {
            GuildId = guildId,
            GuildName = guildName
        });
        bool result = await guildService.LeaveGuildAsync(guildId);

        // Assert
        Assert.True(result);
        var guild = await context.Guilds.FirstOrDefaultAsync(g => g.GuildId == guildId, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Null(guild);
    }

    [Fact]
    public async Task TestGuildCount()
    {
        // Arrange
        await using ApplicationDbContext context = new(_dbContextOptions);
        GuildService guildService = new(context, _nsubLogger);

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
