using System.Collections.Generic;
using Orange.Blazor.DTO.Questions;

namespace Orange.Blazor.DTO.Questions;

public class GuildQuestionGetBatchDto
{
    public ulong GuildId { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public List<GuildQuestionGetDto>? Questions { get; set; }
}