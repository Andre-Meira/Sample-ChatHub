using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Sample.ChatHub.Server.API;

public abstract class BaseHub<T> : Hub<T> where T : class
{
    public string? UserName => Context.User?.FindFirst(ClaimTypes.Name)?.Value;
    public Guid UserId => Guid.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value ?? Guid.Empty.ToString());
    public bool IsAuthenticated => Context.User?.Identity?.IsAuthenticated ?? false;

    public BaseHub()
    {

    }
}
