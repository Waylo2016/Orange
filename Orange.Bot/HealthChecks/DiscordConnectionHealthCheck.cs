using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Orange.Bot.HealthChecks;

public class DiscordConnectionHealthCheck(DiscordSocketClient client) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (client.ConnectionState == ConnectionState.Connected && client.CurrentUser is not null)
        {
            return Task.FromResult(HealthCheckResult.Healthy(
                $"Ready as {client.CurrentUser?.Username ?? "(unknown)"}"));
        }

        var msg = client.ConnectionState switch
        {
            ConnectionState.Connecting => "Connecting",
            ConnectionState.Connected => "Ready",
            ConnectionState.Disconnecting => "Disconnecting",
            ConnectionState.Disconnected => "Disconnected",
            _ => $"Unknown state: {client.ConnectionState}"
        };

        return Task.FromResult(HealthCheckResult.Degraded(msg));
    }
}