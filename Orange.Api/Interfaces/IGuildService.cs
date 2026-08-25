using System.Threading.Tasks;
using Orange.Api.Models;

namespace Orange.Api.Interfaces;

public interface IGuildService
{
    Task<Guild> JoinGuildAsync(ulong id);
    
    Task<bool> LeaveGuildAsync(ulong id);
    
    Task<int> GetGuildCountAsync();
}