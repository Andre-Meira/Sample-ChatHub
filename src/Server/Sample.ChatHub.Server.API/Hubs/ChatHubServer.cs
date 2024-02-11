using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Sample.ChatHub.Domain.Contracts;

namespace Sample.ChatHub.Server.API;

[Authorize]
public class ChatHubServer : Hub
{
    private readonly ILogger<ChatHubServer> _logger;

    public ChatHubServer(ILogger<ChatHubServer> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        string? user = this.Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        _logger.LogInformation("{0} Connected in server", user);

        var context = new ReceiveMessageContext(Guid.NewGuid(), Guid.NewGuid(), "Sistema", $"Bem vindo ao chat {user}");
        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", context);
    }

    public async Task JoineChat(Guid guid)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, guid.ToString());
    }

    public async Task SendMessage(Guid idChat, string message)
    {
        string user = this.Context.User!.FindFirst(ClaimTypes.Name)!.Value;
        Guid guid = Guid.Parse(this.Context.User!.FindFirst(ClaimTypes.Sid)!.Value);
        
        var context = new ReceiveMessageContext(idChat, guid, user, message);
        IClientProxy client = Clients.GroupExcept(idChat.ToString(), Context.ConnectionId);

        await client.SendAsync("ReceiveMessage",context);
    }
}
