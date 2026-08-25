using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Orange.Api.utils;

public class ApplicationDbContextFactory()
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {

        IConfigurationBuilder builder = new ConfigurationBuilder().AddUserSecrets(typeof(ApplicationDbContextFactory).Assembly);
        IConfigurationRoot configuration = builder.Build();
        
        string? postgresUsername = configuration["Parameters:postgres-username"];
        string? postgresPassword = configuration["Parameters:Orange-password"];

        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql($"Host=localhost;Database=OrangeDb;Username={postgresUsername};Password={postgresPassword}");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}