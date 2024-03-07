using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Sample.ChatHub.Domain.Contracts;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Server.API.Protos;

namespace Sample.ChatHub.Server.API.Services;

public class SyncMessageProtoHandler : UserSync.UserSyncBase
{
    private readonly IHubContext<ChatHubServer, IChatHub> _hub;
    private readonly List<User> _usersOptions;

    public SyncMessageProtoHandler(IHubContext<ChatHubServer, IChatHub> hub, 
        IConfiguration configuration)
    {
        _hub = hub;
        _usersOptions = configuration.GetSection(UserSettings.Key).Get<List<User>>()!;
    }

    public override async Task<BoolValue> SyncMessage(SyncMessageRequest request, ServerCallContext context)
    {
        IChatHub userChat = _hub.Clients.User(request.UserId);
        CancellationToken cancellationToken = context.CancellationToken; 

        foreach (MessageList msg in request.Messages)
        {
            Guid chatId = Guid.Parse(msg.IdChat);

            User user = _usersOptions.FirstOrDefault(e => e.Id == msg.SenderId)!;

            var contextMessage = new ContextMessage(chatId, Guid.Parse(msg.SenderId), 
                user.Name, msg.Text, date: DateTime.Parse(msg.DateTime));            

            await userChat.ReceiveMessage(contextMessage);
        }

        return new BoolValue{ Value = true };
    }
}
