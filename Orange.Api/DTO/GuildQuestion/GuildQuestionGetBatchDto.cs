using System.Collections.Generic;

namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionGetBatchDto
{
    public ulong GuildId { get; set; }
    public List<GuildQuestionGetDto>? Questions { get; set; }
}