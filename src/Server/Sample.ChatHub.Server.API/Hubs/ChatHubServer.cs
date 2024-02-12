using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Contracts;
using Sample.ChatHub.Domain.Contracts.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;

namespace Sample.ChatHub.Server.API;

[Authorize]
public class ChatHubServer : BaseHub<IChatHub>
{
    private readonly ILogger<ChatHubServer> _logger;
    private readonly IPublishContext _context;

    public ChatHubServer(ILogger<ChatHubServer> logger, IPublishContext context)
    {
        _logger = logger;
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("{0} Connected in server", UserName);

        var context = new ReceiveMessageContext(Guid.Empty, Guid.Empty, "Sistema", $"Bem vindo ao chat {UserName!}");        
        await Clients.Client(Context.ConnectionId).ReceiveMessage(context);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("{0} User disconnected", UserName);
        return base.OnDisconnectedAsync(exception);
    } 

    public async Task SendMessage(Guid idChat, string message)
    {
        var context = new ReceiveMessageContext(idChat,UserId, UserName!, message);

        var contractMessage = new SendMessage(idChat, UserId, message);
        await _context.PublishMessage(contractMessage).ConfigureAwait(false);

        var client = Clients.GroupExcept(idChat.ToString(), Context.ConnectionId);
        await client.ReceiveMessage(context);
    }
}
