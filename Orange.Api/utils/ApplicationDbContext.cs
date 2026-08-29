using Microsoft.EntityFrameworkCore;
using Orange.Api.Models;

namespace Orange.Api.utils;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<GuildQuestion> GuildQuestions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Guild entity
        modelBuilder.Entity<Guild>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<Guild>()
            .Property(g => g.GuildId)
            .IsRequired()
            .ValueGeneratedNever();

        // Configure the GuildQuestion entity.
        modelBuilder.Entity<GuildQuestion>()
            .HasAlternateKey(gq => gq.GuildId);
        
        modelBuilder.Entity<GuildQuestion>()
            .Property(gq => gq.GuildId)
            .IsRequired()
            .ValueGeneratedNever();

        // 1-* relationship between Guild and GuildQuestion
        modelBuilder.Entity<Guild>()
            .HasMany(g => g.GuildQuestions)
            .WithOne(gq => gq.Guild)
            .HasPrincipalKey(g => g.GuildId)
            .HasForeignKey(gq => gq.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}