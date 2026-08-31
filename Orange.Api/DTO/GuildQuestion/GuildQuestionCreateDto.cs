namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionCreateDto
{
    public ulong GuildId { get; set; }
    
    public required string Question { get; set; }
    
    public int QuestionOrder { get; set; }
}