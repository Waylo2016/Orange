using System.Threading.Tasks;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Handlers;

public class GuildJoinHandler(IGuildEvents guildEvents) : IGuildCreateGatewayHandler
{
    public async ValueTask HandleAsync(GuildCreateEventArgs arg)
    {
        if (arg.Guild is null)
        {
            return;
        }
        
        await guildEvents.OnGuildJoining(arg.Guild.Id, arg.Guild.Name);
    }
}