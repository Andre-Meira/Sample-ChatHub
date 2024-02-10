using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace Sample.ChatHub.Server.API;

public class DefaultController : ControllerBase
{
    protected Guid UserID => Guid.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
    protected string? UserName => User.FindFirst(ClaimTypes.Name)?.Value;
}
