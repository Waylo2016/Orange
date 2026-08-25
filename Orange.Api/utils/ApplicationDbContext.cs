using Microsoft.EntityFrameworkCore;
using Orange.Api.Models;

namespace Orange.Api.utils;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Guild> Guilds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Guild entity, it has no relationships, so we only need to configure the PK and the required field.
        modelBuilder.Entity<Guild>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<Guild>()
            .Property(g => g.GuildId)
            .IsRequired();
    }
}