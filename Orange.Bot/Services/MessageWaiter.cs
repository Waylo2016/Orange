using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Services;

public class MessageWaiter(DiscordSocketClient client, ILogger<MessageWaiter> logger) : IMessageWaiter
{
    /// <summary>
    /// waits for a message from a specific user in a specific channel, with a timeout and a cancellation token
    /// </summary>
    /// <param name="userId">the userId of the user to wait for</param>
    /// <param name="channelId">the channelId of the user to wait for</param>
    /// <param name="timeout">timeout preferred in seconds</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>
    /// The first message sent by <paramref name="userId"/> in the channel
    /// identified by <paramref name="channelId"/> within the specified <paramref name="timeout"/>,
    /// or <c>null</c> if no message is received within the timeout period or if the operation is canceled.
    /// </returns>
    public async Task<SocketMessage?> WaitForMessageAsync(
        ulong userId, 
        ulong channelId, 
        TimeSpan timeout, 
        CancellationToken cancellationToken)
    {
        
        // Create a TaskCompletionSource to await the message
        var tcs = new TaskCompletionSource<SocketMessage?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        
        // Define a local function to handle incoming messages
        Task Handler(SocketMessage message)
        {
            if (message.Author.Id == userId && message.Channel.Id == channelId)
            {
                tcs.TrySetResult(message);
            }
            return Task.CompletedTask;
        }

        // Subscribe to the MessageReceived event
        client.MessageReceived += Handler;

        // Create a linked cancellation token source to handle both the provided cancellation token and the timeout
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            // Wait for the message or timeout/cancellation
            return await tcs.Task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Log the timeout event
            logger.LogInformation("[{Source}] | Timeout waiting for message from user {UserId} in channel {ChannelId}", "MessageWaiter",  userId, channelId);
            return null;
        }
        finally
        {
            // Unsubscribe from the MessageReceived event to avoid memory leaks
            client.MessageReceived -= Handler;
        }
    }
}