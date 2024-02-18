using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Sample.ChatHub.Woker.API.Protos;

namespace Sample.ChatHub.Server.API;

public interface IUserService
{
    Task<UserChats> GetUserChats(Guid idUser);        
}

class UserHandler : IUserService
{
    private IDistributedCache _cache;    
    private readonly UserInfo.UserInfoClient _userClient;

    public UserHandler(IDistributedCache cache, 
        
        UserInfo.UserInfoClient userClient)
    {
        _cache = cache;
        _userClient = userClient;
    }

    public async Task<UserChats> GetUserChats(Guid idUser) 
    {
        string? json = await _cache.GetStringAsync(idUser.ToString());

        if (json is not null)
        {
            return JsonConvert.DeserializeObject<UserChats>(json)!;
        }
        
        UserChats user = await SendRequestAsync(idUser);

        return user;
    }

    private async Task<UserChats> SendRequestAsync(Guid idUser)
    {
        var userChat = new UserChats(idUser);
        var request = new UserInfoRequest{ UserId = idUser.ToString() };
        
        UserChatsReponse reponse = await _userClient.GetUserChatsAsync(request)
            .ConfigureAwait(false);

        foreach (string chat in reponse.ChatsID)
        {
            userChat.IdChats.Add(Guid.Parse(chat));
        }

        return userChat;
    }
}


