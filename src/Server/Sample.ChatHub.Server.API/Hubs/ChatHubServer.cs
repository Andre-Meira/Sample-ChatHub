using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Contracts;
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
        _logger.LogInformation("{0} Connected from the server.", UserName);
        Guid guid = Guid.Parse("07718859-6209-464e-891c-c761035d9980");

        await Groups.AddToGroupAsync(Context.ConnectionId, guid.ToString());

        var context = new ContextMessage(Guid.Empty, Guid.Empty, "Sistema",$"Bem vindo ao chat {UserName!}");        
        await Clients.Client(Context.ConnectionId).ReceiveMessage(context);

        await _context.PublishMessage<SyncUserMessage>(new(UserId));
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("{0} Disconnected from the server.", UserName);
        return base.OnDisconnectedAsync(exception);
    } 

    public async Task SendMessage(Guid idChat, string message)
    {        
        var messageContext = new ContextMessage(idChat, UserId, UserName!,message);
        var contractMessage = new SendMessage(messageContext);

        await _context.PublishMessage(contractMessage).ConfigureAwait(false);

        IChatHub client = Clients.GroupExcept(idChat.ToString(), Context.ConnectionId);
        await client.ReceiveMessage(messageContext);
    }

    public async Task AckMessage(Guid IdChat, Guid IdMessage)    
       => await _context.PublishMessage<MessageReceived>(new(IdChat, IdMessage, UserId))
            .ConfigureAwait(false);
    
}
