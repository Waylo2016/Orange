using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orange.Api.Models;


public class Guild
{
    public int Id { get; set; }

    public ulong GuildId { get; set; }

    [MaxLength(100)]
    public string GuildName { get; set; } = string.Empty;

    public ICollection<GuildQuestion> GuildQuestions { get; set; } = new List<GuildQuestion>();
}