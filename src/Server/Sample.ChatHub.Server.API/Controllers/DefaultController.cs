using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Sample.ChatHub.Server.API;

public class DefaultController : ControllerBase
{
    protected Guid UserID => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    protected string? UserName => User.FindFirst(ClaimTypes.Name)?.Value;
}
