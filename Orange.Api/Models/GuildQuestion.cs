namespace Orange.Api.Models;

public class GuildQuestion
{
    public int Id { get; set; }

    public ulong GuildId { get; set; }

    public string Question { get; set; } = string.Empty;

    public int QuestionOrder { get; set; }

    public Guild Guild { get; set; } = null!;
}