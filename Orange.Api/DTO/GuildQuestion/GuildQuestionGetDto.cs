namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionGetDto
{
    public ulong GuildId { get; set; }
    public int QuestionOrder { get; set; }
    public required string Question { get; set; }
}