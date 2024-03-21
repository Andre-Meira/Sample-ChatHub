using Google.Protobuf.WellKnownTypes;
using Sample.ChatHub.Server.API.Protos;
using Sample.ChatHub.Worker.Core.Messages;

namespace Sample.ChatHub.Worker.API.Services;

internal sealed class SyncMessageService
{
    private readonly UserSync.UserSyncClient _userSyncClient;

    public SyncMessageService(UserSync.UserSyncClient userSync)
    {
        _userSyncClient = userSync;
    }

    public async Task<bool> SyncMessagen(Guid userID, List<MessageHub> messageHubs)
    {
        SyncMessageRequest syncMessageRequest = new SyncMessageRequest();
        syncMessageRequest.UserId = userID.ToString();

        foreach (MessageHub messages in messageHubs)
        {
            MessageList messageList = new MessageList()
            {
                IdChat = messages.ChatId.ToString(),
                MessageId = messages.MessageId.ToString(),
                SenderId = messages.SenderId.ToString(),
                Text = messages.Message,
                DateTime = messages.Timestamp.ToString()
            };

            syncMessageRequest.Messages.Add(messageList);
        }


        BoolValue @bool = await _userSyncClient.SyncMessageAsync(syncMessageRequest);

        return @bool.Value;
    }


}
