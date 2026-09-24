using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetCord.Gateway;

namespace Orange.Bot.HealthChecks;

public class DiscordConnectionHealthCheck(GatewayClient client) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (client.Status == WebSocketStatus.Ready)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Ready"));
        }
        
        var msg = client.Status switch
        {
            WebSocketStatus.Connecting => "Connecting",
            WebSocketStatus.Ready => "Ready",
            WebSocketStatus.Disconnected => "Disconnected",
            _ => $"Unknown state: {client.Status}"
        };

        return Task.FromResult(HealthCheckResult.Degraded(msg));
    }
}