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
        var discordDevGuildId = builder.AddParameter("DevGuildId", secret: true);
        var postgresUsername = builder.AddParameter("postgres-username", secret: true);

        var seq = builder.AddSeq("seq")
            .ExcludeFromManifest()
            .WithContainerName("seq")
            .WithContainerRuntimeArgs("--label", stackLabel)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithHttpEndpoint(port: 5341, targetPort: 80, name: "http")
            .WithEnvironment("ACCEPT_EULA", "Y");

        var postgres = builder.AddPostgres("OrangePostgres", postgresUsername)
            .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050)
                .WithContainerName("pgadmin")
                .WithContainerRuntimeArgs("--label", stackLabel)
                .WithLifetime(ContainerLifetime.Persistent))
            .WithContainerName("postgres")
            .WithContainerRuntimeArgs("--label", stackLabel)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume(isReadOnly: false);

        var postgresdb = postgres.AddDatabase("OrangeDb");

        var api = builder.AddProject<Projects.Orange_Api>("api")
            .WithHttpEndpoint(8080, name: "http")
            .WithReference(seq)
            .WithReference(postgresdb)
            .WaitFor(postgresdb);

        // var apiMigrations = api.AddEFMigrations("api-migrations");

        var blazorApp = builder.AddBlazorWasmProject<Projects.Orange_Blazor>("web-dashboard")
            .WithReference(api)
            .WithReference(seq);

        var gateway = builder.AddBlazorGateway("gateway")
            .WithExternalHttpEndpoints();

        var bot = builder.AddProject<Projects.Orange_Bot>("discord-bot")
            .WithEnvironment("Discord__Api__Key", discordApiKey)
            .WithEnvironment("Discord__Client__Id", discordClientId)
            .WithEnvironment("Discord__DevGuildId", discordDevGuildId)
            .WithReference(seq)
            .WithReference(api.GetEndpoint("http"))
            .WaitFor(api);

        gateway.WithBlazorClientApp(blazorApp);

        builder.Build().Run();
    }
}