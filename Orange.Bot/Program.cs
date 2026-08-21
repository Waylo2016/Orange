using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Orange.Bot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}
