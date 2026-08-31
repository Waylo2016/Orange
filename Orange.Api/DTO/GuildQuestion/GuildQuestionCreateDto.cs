namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionCreateDto
{
    public ulong GuildId { get; set; }

    public string Question { get; set; }

    public int QuestionOrder { get; set; }
}