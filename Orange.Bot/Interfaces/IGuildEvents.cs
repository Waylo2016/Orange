using System.Threading.Tasks;
using NetCord.Rest;

namespace Orange.Bot.Interfaces;

public interface IGuildEvents
{
    /// <summary>
    /// Called when the bot joins a guild
    /// </summary>
    /// <param name="guildId">The ID of the guild that was joined</param>
    /// <param name="guildName">The name of the guild that was joined</param>
    /// <returns>A call to the Orange API adding the guild to the Database</returns>
    public Task OnGuildJoining(ulong guildId, string guildName);

    /// <summary>
    /// Called when the bot leaves a guild
    /// </summary>
    /// <param name="guildId">The ID of the guild that was left</param>
    /// <returns>A call to the Orange API removing the guild from the database</returns>
    public Task OnGuildLeave(ulong guildId);
}