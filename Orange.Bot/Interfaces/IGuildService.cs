using System.Threading.Tasks;

namespace Orange.Bot.Interfaces;

/// <summary>
/// Represents a service for managing guild-related operations.
/// </summary>
public interface IGuildService
{
    /// <summary>
    /// Registers a guild with the specified guild ID.
    /// </summary>
    /// <param name="guildId">The ID of the guild to register.</param>
    Task RegisterGuildAsync(ulong guildId);

    /// <summary>
    /// Unregisters a guild with the specified guild ID.
    /// </summary>
    /// <param name="guildId">The ID of the guild to unregister.</param>
    Task UnregisterGuildAsync(ulong guildId);
}