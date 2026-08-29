namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionUpdateDto
{
    public ulong GuildId { get; set; }
    
    public string Question { get; set; }

    public int QuestionOrder { get; set; }
}