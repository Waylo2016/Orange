using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Orange.Bot.Interfaces;

public interface IMessageWaiter
{
    public Task<SocketMessage?> WaitForMessageAsync(
        ulong userId, 
        ulong channelId, 
        TimeSpan timeout, 
        CancellationToken cancellationToken = default
        );
}