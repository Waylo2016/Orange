namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionGetDto
{
    public ulong GuildId { get; set; }
    public int QuestionOrder { get; set; }
    public string Question { get; set; }
}