using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orange.Api.utils;

namespace Orange.Api.MigrationService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "OrangeDb");

        var host = builder.Build();
        host.Run();
    }
}
