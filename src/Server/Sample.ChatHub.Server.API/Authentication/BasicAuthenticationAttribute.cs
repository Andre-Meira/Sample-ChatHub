using Microsoft.AspNetCore.Authorization;

namespace Sample.ChatHub.Server.API;

public class BasicAuthenticationAttribute : AuthorizeAttribute
{
    public BasicAuthenticationAttribute()
    {
        this.AuthenticationSchemes = "Basic";
    }
}
