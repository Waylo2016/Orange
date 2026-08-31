using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Orange.Api.utils;

namespace xUnit.Tests.Helpers;

/// <summary>
/// Backs <see cref="ApplicationDbContext"/> with a SQLite in-memory database instead of a
/// Testcontainers Postgres instance. Unlike the EF Core InMemory provider, SQLite is a real
/// relational engine, so foreign keys, alternate keys and unique indexes are actually enforced -
/// the whole point being to catch relationship bugs that InMemory silently lets through.
/// </summary>
public sealed class SqliteFixture : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    public async ValueTask InitializeAsync()
    {
        // ":memory:" only survives for as long as a connection to it stays open, so we keep
        // this one alive for the fixture's lifetime and hand it to every DbContext we create.
        _connection = new SqliteConnection("Filename=:memory:");
        await _connection.OpenAsync();

        await using var db = CreateDbContext();
        await db.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync() => await _connection.DisposeAsync();

    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new ApplicationDbContext(options);
    }

    /// <summary>
    /// Wipes every row so the next test starts from an empty database, without paying to spin
    /// up a fresh container/schema per test the way PostgresFixture would have. Call this from
    /// the test class's own IAsyncLifetime.InitializeAsync (per-test setup) rather than the
    /// fixture's (per-class setup) to get isolation between [Fact]s that share this fixture.
    /// </summary>
    public async Task ResetAsync()
    {
        await using var db = CreateDbContext();

        var tableNames = db.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Where(name => name is not null)
            .Distinct();

        await db.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");
        foreach (var table in tableNames)
        {
            // Table names come from our own model metadata, not user input, so building the
            // statement this way (rather than ExecuteSqlInterpolated) is safe here - SQL
            // parameters can't stand in for identifiers anyway.
            string deleteSql = "DELETE FROM \"" + table + "\";";
            await db.Database.ExecuteSqlRawAsync(deleteSql);
        }
        await db.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");
    }
}
