using Grpc.Core;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Woker.API.Protos;

namespace Sample.ChatHub.Worker.API.Services;

public sealed class UserInfoProtoHandler : UserInfo.UserInfoBase
{
    private readonly IChatProcessStream _chatProcess;

    public UserInfoProtoHandler(IChatProcessStream chatProcess) => _chatProcess = chatProcess;

    public override async Task<UserChatsReponse> GetUserChats(UserInfoRequest request, ServerCallContext context)
    {
        Guid userId = Guid.Parse(request.UserId);
        IEnumerable<Guid> userChats = await _chatProcess.GetChatByUser(userId);

        var response = new UserChatsReponse();

        foreach (var chat in userChats) response.ChatsID.Add(chat.ToString());

        return response;
    }

}

