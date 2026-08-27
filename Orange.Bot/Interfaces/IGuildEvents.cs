using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Orange.Bot.Interfaces;

public interface IGuildEvents
{
    /// <summary>
    /// Called when the bot joins a guild
    /// </summary>
    /// <param name="guild">websocket instance</param>
    /// <returns>A call to the Orange API adding the guild to the Database</returns>
    public Task OnGuildJoining(IGuild guild);

    /// <summary>
    /// Called when the bot leaves a guild
    /// </summary>
    /// <param name="guild">websocket instance </param>
    /// <returns>A call to the Orange API removing the guild from the database</returns>
    public Task OnGuildLeave(IGuild guild);
}