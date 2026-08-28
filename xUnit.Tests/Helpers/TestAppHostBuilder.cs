using Aspire.Hosting;
using Microsoft.Extensions.Logging;
using Projects;
using Xunit.Abstractions;

namespace xUnit.Tests.Helpers;

/// <summary>
/// A builder class for creating a test application host with optional components such as API, Bot, and Dashboard. This class allows for configuring the test environment and logging output for integration tests.
/// </summary>
public sealed class TestAppHostBuilder
{
    private bool _includeApi;
    private bool _includeBot;
    private bool _includeDashboard;
    private ITestOutputHelper? _output;
    private TimeSpan _timeout;

    /// <summary>
    /// Includes the API component in the test application host.
    /// </summary>
    public TestAppHostBuilder WithApi()
    {
        _includeApi = true;
        return this;
    }

    /// <summary>
    /// Includes the Bot component in the test application host. This also includes the API component, as the Bot depends on it.
    /// <see cref="WithApi"/>
    /// </summary>
    public TestAppHostBuilder WithBot()
    {
        _includeBot = true;
        _includeApi = true;
        return this;
    }

    /// <summary>
    /// Includes the Dashboard component in the test application host. This also includes the API component, as the Dashboard depends on it.
    /// <see cref="WithApi"/>
    /// </summary>
    /// <returns></returns>
    public TestAppHostBuilder WithDashboard()
    {
        _includeDashboard = true;
        _includeApi = true;
        return this;
    }

    /// <summary>
    /// Configures the test application host to log output to the provided <see cref="ITestOutputHelper"/> instance. This is useful for capturing logs during integration tests.
    /// </summary>
    /// <param name="output">The <see cref="ITestOutputHelper"/> instance to use for logging.</param>
    /// <returns></returns>
    public TestAppHostBuilder LogTo(ITestOutputHelper output) { _output = output; return this; }


    /// <summary>
    /// Sets the timeout duration for starting the test application host. If the host fails to start within this period,
    /// a <see cref="TimeoutException"/> will be thrown.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public TestAppHostBuilder WithTimeout(TimeSpan timeout)
    {
        _timeout = timeout;
        return this;
    }

    /// <summary>
    /// Builds and starts the test application host asynchronously,
    /// returning a <see cref="DistributedApplication"/> instance.
    /// The host will include the specified components (API, Bot, Dashboard) and will log output if configured.
    /// </summary>
    /// <param name="ct">cancellation token</param>
    /// <returns>the started app</returns>
    /// <exception cref="TimeoutException">thrown when the application fails to start within the timeout period
    /// <see cref="WithTimeout"/>
    /// </exception>
    public async Task<DistributedApplication> BuildAsync
        (CancellationToken ct = default)
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Orange_AppHost>(
                [
                    "Test:Enabled=true",
                    $"Test:IncludeApi={_includeApi}",
                    $"Test:IncludeBot={_includeBot}",
                    $"Test:IncludeDashboard={_includeDashboard}",
                    "Test:StackLabel=com.docker.compose.project=Orange-TEST"
                ],
                ct
                );

        appHost.Configuration["Test:Enabled"] = "true";
        appHost.Configuration["Test:IncludeApi"] = _includeApi.ToString();
        appHost.Configuration["Test:IncludeBot"] = _includeBot.ToString();
        appHost.Configuration["Test:IncludeDashboard"] = _includeDashboard.ToString();
        appHost.Configuration["Test:StackLabel"] = "com.docker.compose.project=Orange-TEST";

        if (_output is not null)
        {
            appHost.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                logging.AddXUnit(_output);
            });
        }

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        var app = await appHost.BuildAsync(ct).WaitAsync(_timeout, ct);

        if (_output is not null)
        {
            StreamResourceLogsToOutput(app, _output, ct);
        }

        await app.StartAsync(ct).WaitAsync(_timeout, ct);

        return app;
    }

    private static void StreamResourceLogsToOutput(
        DistributedApplication app,
        ITestOutputHelper output,
        CancellationToken cancellationToken)
    {
        var resourceLogger = app.Services.GetRequiredService<ResourceLoggerService>();
        var resourceNotifications = app.Services.GetRequiredService<ResourceNotificationService>();
        var watchedResources = new HashSet<string>();
        var watchedLock = new object();

        _ = Task.Run(async () =>
        {
            try
            {
                await foreach (var resourceEvent in resourceNotifications
                    .WatchAsync(cancellationToken))
                {
                    var resourceName = resourceEvent.Resource.Name;

                    // Dedupe: watch elke resource maar één keer, ook al krijgen we
                    // meerdere state-updates voor 'm binnen.
                    lock (watchedLock)
                    {
                        if (!watchedResources.Add(resourceName)) continue;
                    }

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await foreach (var logEvent in resourceLogger
                                .WatchAsync(resourceName)
                                .WithCancellation(cancellationToken))
                            {
                                foreach (var line in logEvent)
                                {
                                    output.WriteLine($"[{resourceName}] {line.Content}");
                                }
                            }
                        }
                        catch (OperationCanceledException) { /* clean shutdown */ }
                        catch (InvalidOperationException)
                        {
                            // Kan gebeuren als xUnit de test al gemarkeerd heeft als klaar
                            // en de ITestOutputHelper niet meer accepteert. Slikken.
                        }
                    }, cancellationToken);
                }
            }
            catch (OperationCanceledException) { /* clean shutdown */ }
        }, cancellationToken);
    }
}
