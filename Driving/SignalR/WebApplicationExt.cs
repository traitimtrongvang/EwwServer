using Microsoft.AspNetCore.Builder;
using SignalR.Hubs.User;

namespace SignalR;

public static class WebApplicationExt
{
    public static void UseSignalRConfigurations(this WebApplication app)
    {
        app.MapHub<ConversationHub>("/hub/conversations");
    }
}