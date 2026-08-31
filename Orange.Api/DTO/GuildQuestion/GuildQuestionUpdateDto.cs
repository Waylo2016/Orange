namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionUpdateDto
{
    public ulong GuildId { get; set; }

    public string NewQuestion { get; set; }

    public int OldQuestionOrder { get; set; }

    public int NewQuestionOrder { get; set; }
}