using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Postgres;

namespace Orange.AppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);
        builder.AddDockerComposeEnvironment("Orange");
        
        const string stackLabel = "com.docker.compose.project=Orange";

        var discordApiKey = builder.AddParameter("DiscordApiKey", secret: true);
        var discordClientId = builder.AddParameter("DiscordClientId", secret: true);

        var seq = builder.AddSeq("seq")
            .ExcludeFromManifest()
            .WithContainerName("seq")
            .WithContainerRuntimeArgs("--label", stackLabel)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithEnvironment("ACCEPT_EULA", "Y");

        var postgres = builder.AddPostgres("postgres")
            .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050)
                .WithContainerName("pgadmin")
                .WithContainerRuntimeArgs("--label", stackLabel)
                .WithLifetime(ContainerLifetime.Persistent))
            .WithContainerName("postgres")
            .WithContainerRuntimeArgs("--label", stackLabel)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume();
        
        var postgresdb = postgres.AddDatabase("postgresdb");

        var api = builder.AddProject<Projects.Orange_Api>("api")
            .WithHttpEndpoint(8080)
            .WithReference(seq)
            .WithReference(postgresdb)
            .WaitFor(postgresdb);

        var apiMigrations = api.AddEFMigrations("api-migrations");

        var blazorApp = builder.AddBlazorWasmProject<Projects.Orange_Blazor>("web-dashboard")
            .WithReference(api)
            .WithReference(seq);

        var gateway = builder.AddBlazorGateway("gateway")
            .WithExternalHttpEndpoints();
        
        var bot = builder.AddProject<Projects.Orange_Bot>("discord-bot")
            .WithEnvironment("Discord__Api__Key", discordApiKey)
            .WithEnvironment("Discord__Client__Id", discordClientId)
            .WithReference(seq)
            .WithReference(api.GetEndpoint("http"))
            .WaitFor(api);

        gateway.WithBlazorClientApp(blazorApp);
        
        builder.Build().Run();
    }
}