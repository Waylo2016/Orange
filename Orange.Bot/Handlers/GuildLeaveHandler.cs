using System.Threading.Tasks;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using Orange.Bot.Interfaces;

namespace Orange.Bot.Handlers;

public class GuildLeaveHandler(IGuildEvents guildEvents) : IGuildDeleteGatewayHandler
{
    public async ValueTask HandleAsync(GuildDeleteEventArgs arg)
    {
        if (arg.IsUnavailable)
        {
            return;
        }

        await guildEvents.OnGuildLeave(arg.GuildId);
    }
}