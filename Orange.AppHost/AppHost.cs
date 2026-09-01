using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;

namespace Orange.AppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);
        builder.AddDockerComposeEnvironment("Orange");

        const string stackLabel = "com.docker.compose.project=Orange";

        // test mode config, default = false
        var testMode = builder.Configuration.GetValue("Test:Enabled", false);
        var includeApi = !testMode || builder.Configuration.GetValue("Test:IncludeApi", false);
        var includeBot = !testMode || builder.Configuration.GetValue("Test:IncludeBot", false);
        var includeDashboard = !testMode || builder.Configuration.GetValue("Test:IncludeDashboard", false);
        var stackLabelFromConfig = testMode ? builder.Configuration.GetValue("Test:StackLabel", stackLabel) : stackLabel;
        


        // parameters
        var discordApiKey = builder.AddParameter("DiscordApiKey", secret: true);
        var discordClientId = builder.AddParameter("DiscordClientId", secret: true);
        var discordDevGuildId = builder.AddParameter("DevGuildId", secret: true);
        var postgresUsername = builder.AddParameter("postgres-username", secret: true);

        var seq = builder.AddSeq("seq")
            .ExcludeFromManifest()
            .WithContainerName("seq")
            .WithContainerRuntimeArgs("--label", stackLabelFromConfig)
            .WithLifetime(ContainerLifetime.Session)
            .WithHttpEndpoint(port: 5341, targetPort: 80, name: "http")
            .WithEnvironment("ACCEPT_EULA", "Y");


        // Postgres database
        IResourceBuilder<PostgresDatabaseResource>? postgresdb = null;
        if (includeApi)
        {
            var postgres = builder.AddPostgres("Orange", postgresUsername)
                .WithPgAdmin(pgAdmin =>
                {
                    pgAdmin.WithHostPort(5050)
                        .WithContainerRuntimeArgs("--label", stackLabelFromConfig);

                    if (!testMode)
                        pgAdmin.WithContainerName("pgadmin")
                            .WithLifetime(ContainerLifetime.Persistent);
                })
                .WithContainerRuntimeArgs("--label", stackLabelFromConfig);



            if (!testMode)
                postgres.WithContainerName("postgres")
                    .WithHostPort(5432)
                    .WithDataVolume()
                    .WithLifetime(ContainerLifetime.Persistent);

            postgresdb = postgres.AddDatabase("OrangeDb");
        }


        // API
        IResourceBuilder<ProjectResource>? api = null;
        if (includeApi)
        {
            api = builder.AddProject<Projects.Orange_Api>("orange-api")
                .WithHttpEndpoint(8080, name: "http")
                .WithReference(seq);

            if (postgresdb is not null)
            {
                var apiMigrations = builder.AddProject<Projects.Orange_Api_MigrationService>("api-migrations")
                    .WithReference(postgresdb)
                    .WaitFor(postgresdb);

                api = api.WithReference(postgresdb)
                    .WaitFor(postgresdb)
                    .WaitForCompletion(apiMigrations);
            }
        }

        // Dashboard + gateway
        if (includeDashboard && api is not null)
        {
            var blazorApp = builder.AddBlazorWasmProject<Projects.Orange_Blazor>("web-dashboard")
                .WithReference(api)
                .WithReference(seq);

            var gateway = builder.AddBlazorGateway("blazor-gateway")
                .WithExternalHttpEndpoints();

            gateway.WithBlazorClientApp(blazorApp);
        }

        // Bot
        if (includeBot && api is not null)
        {
            builder.AddProject<Projects.Orange_Bot>("discord-bot")
                .WithHttpEndpoint(port: 8081, name: "http")
                .WithHttpHealthCheck("/health")
                .WithEnvironment("Discord__Api__Key", discordApiKey)
                .WithEnvironment("Discord__Client__Id", discordClientId)
                .WithEnvironment("Discord__DevGuildId", discordDevGuildId)
                .WithReference(seq)
                .WithReference(api)
                .WaitFor(api);
        }
        

        builder.Build()
            .Run();
    }
}