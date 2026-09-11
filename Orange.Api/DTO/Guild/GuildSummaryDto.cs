namespace Orange.Api.DTO.Guild;

public class GuildSummaryDto
{
    public ulong GuildId { get; set; }

    public string GuildName { get; set; } = string.Empty;

    // TODO: decide what else a dashboard user needs per guild (icon, their role/permissions, etc.). See TODO.md.
}
