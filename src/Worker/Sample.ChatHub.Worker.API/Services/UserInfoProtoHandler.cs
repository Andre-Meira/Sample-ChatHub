using Grpc.Core;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Woker.API.Protos;

namespace Sample.ChatHub.Worker.API.Services;

public sealed class UserInfoProtoHandler : UserInfo.UserInfoBase
{
    private readonly IChatEventsRepositore _chatRepositore;

    public UserInfoProtoHandler(IChatEventsRepositore chatRepositore)
    {
        _chatRepositore = chatRepositore;
    }

    public override async Task<UserChatsReponse> GetUserChats(UserInfoRequest request, ServerCallContext context)
    { 
        Guid userId = Guid.Parse(request.UserId);
        IEnumerable<Guid> userChats = await _chatRepositore.GetUserChats(userId);

        var response = new UserChatsReponse();
        
        foreach (var chat in userChats)
        {
            response.ChatsID.Add(chat.ToString());
        }

        return response;
    }

}

