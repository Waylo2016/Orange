using System.ComponentModel.DataAnnotations;

namespace Orange.Api.Models;


public class Guild
{
    public int Id { get; set; }
    
    public ulong GuildId { get; set; }
}