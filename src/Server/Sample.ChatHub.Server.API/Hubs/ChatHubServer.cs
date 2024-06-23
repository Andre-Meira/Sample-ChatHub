using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Polly;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Contracts;
using Sample.ChatHub.Domain.Contracts.Messages;

namespace Sample.ChatHub.Server.API;

[Authorize]
public class ChatHubServer : BaseHub<IChatHub>
{
    private readonly ILogger<ChatHubServer> _logger;
    private readonly IPublishContext _context;
    private readonly IUserService _userService;

    public ChatHubServer(ILogger<ChatHubServer> logger,
        IPublishContext context, IUserService userService)
    {
        _logger = logger;
        _context = context;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            _logger.LogInformation("{0} Connected from the server.", UserName);
            await this.SetChannelAsync();

            var context = new ContextMessage(Guid.Empty, Guid.Empty, "Sistema", $"Bem vindo ao chat {UserName!}");
            await Clients.Client(Context.ConnectionId).ReceiveMessage(context);

            await _context.PublishMessage<SyncUserMessage>(new(UserId));
        }
        catch (Exception err)
        {
            var context = new ContextMessage(Guid.Empty, Guid.Empty, "Sistema", "Nao foi possivel sincronizar o usuario.");
            await Clients.Client(Context.ConnectionId).ReceiveMessage(context);

            _logger.LogError("Falha ao sincronizar o usuario erro: {0}", err.Message);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("{0} Disconnected from the server.", UserName);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Guid idChat, string message)
    {
        var messageContext = new ContextMessage(idChat, UserId, UserName!, message);
        var contractMessage = new SendMessage(messageContext);

        UserChats userChats = await _userService.GetUserChats(UserId);

        if (userChats.IdChats.Contains(idChat.ToString()) == false)
        {
            var context = new ContextMessage(Guid.Empty, Guid.Empty, "Sistema", $"Você não possui acesso a esse chat.");
            await Clients.Client(Context.ConnectionId).ReceiveMessage(context);

            return;
        }

        await _context.PublishMessage(contractMessage).ConfigureAwait(false);

        IChatHub client = Clients.GroupExcept(idChat.ToString(), Context.ConnectionId);
        await client.ReceiveMessage(messageContext);
    }

    public async Task AckMessage(Guid IdChat, Guid IdMessage)
       => await _context.PublishMessage<MessageReceived>(new(IdChat, IdMessage, UserId))
            .ConfigureAwait(false);


    private async Task SetChannelAsync()
    {
        UserChats userChats = await _userService.GetUserChats(UserId);

        foreach (string chat in userChats.IdChats)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chat);
        }
    }
}
