using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Orange.Bot.Interfaces;

public interface IMessageWaiter
{
    
    /// <summary>
    /// waits for a message from a specific user in a specific channel, with a timeout and a cancellation token
    /// </summary>
    /// <param name="userId">the userId of the user to wait for</param>
    /// <param name="channelId">the channelId of the user to wait for</param>
    /// <param name="timeout">timeout preferred in minutes</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>
    /// The first message sent by <paramref name="userId"/> in the channel
    /// identified by <paramref name="channelId"/> within the specified <paramref name="timeout"/>,
    /// or <c>null</c> if no message is received within the timeout period or if the operation is canceled.
    /// </returns>
    public Task<SocketMessage?> WaitForMessageAsync(
        ulong userId, 
        ulong channelId, 
        TimeSpan timeout, 
        CancellationToken cancellationToken = default
        );
}