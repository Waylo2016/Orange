namespace Orange.AppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);
        builder.AddDockerComposeEnvironment("env");

        var seq = builder.AddSeq("seq")
            .ExcludeFromManifest()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithEnvironment("ACCEPT_EULA", "Y");

        var postgres = builder.AddPostgres("postgres")
            .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050))
            .WithDataVolume();
        
        var postgresdb = postgres.AddDatabase("postgresdb");

        var api = builder.AddProject<Projects.Orange_Api>("api")
            .WithHttpEndpoint(8080)
            .WithReference(seq)
            .WithReference(postgresdb)
            .WaitFor(postgresdb);

        var blazorApp = builder.AddBlazorWasmProject<Projects.Orange_Blazor>("web-dashboard")
            .WithReference(api)
            .WithReference(seq);

        var gateway = builder.AddBlazorGateway("gateway")
            .WithExternalHttpEndpoints();
        
        var bot = builder.AddProject<Projects.Orange_Bot>("discord-bot")
            .WithReference(seq)
            .WithReference(api.GetEndpoint("http"))
            .WaitFor(api);

        gateway.WithBlazorClientApp(blazorApp);
        
        builder.Build().Run();
    }
}