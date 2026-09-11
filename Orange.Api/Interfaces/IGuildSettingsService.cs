using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.GuildSettings;

namespace Orange.Api.Interfaces;

public interface IGuildSettingsService
{
    public Task<ActionResult<GuildSettingsDto>> GetGuildSettingsAsync(ulong guildId);

    public Task<ActionResult<GuildSettingsDto>> UpdateGuildSettingsAsync(ulong guildId, GuildSettingsUpdateDto guildSettings);
}