using System.Formats.Asn1;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Sample.ChatHub.Woker.API.Protos;

namespace Sample.ChatHub.Server.API;

public interface IUserService
{
    Task<UserChats> GetUserChats(Guid idUser, CancellationToken cancellationToken = default);
    Task IncludeUserChat(Guid idUser, Guid idChat, CancellationToken cancellationToken = default);
}

class UserService : IUserService
{
    private IDistributedCache _cache;    
    private readonly UserInfo.UserInfoClient _userClient;

    private DistributedCacheEntryOptions CacheEntryOptions = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
    

    public UserService(IDistributedCache cache, 
        
        UserInfo.UserInfoClient userClient)
    {
        _cache = cache;
        _userClient = userClient;
    }

    public async Task<UserChats> GetUserChats(Guid idUser, 
        CancellationToken cancellationToken = default) 
    {
        string? json = await _cache.GetStringAsync(idUser.ToString());

        if (json is not null)
        {
            return JsonConvert.DeserializeObject<UserChats>(json)!;
        }
        
        UserChats user = await SendRequestAsync(idUser);

        string jsonChat = JsonConvert.SerializeObject(user);
        await _cache.SetStringAsync(idUser.ToString(), jsonChat, CacheEntryOptions);

        return user;
    }    

    public async Task IncludeUserChat(Guid idUser, Guid idChat, 
        CancellationToken cancellationToken = default)
    {
        UserChats userChats = await GetUserChats(idUser).ConfigureAwait(false);

        if (userChats.IdChats.Contains(idChat.ToString())) return;

        userChats.IdChats.Add(idChat.ToString());

        string json = JsonConvert.SerializeObject(userChats);
        await _cache.SetStringAsync(idUser.ToString(), json, CacheEntryOptions);
    }


    private async Task<UserChats> SendRequestAsync(Guid idUser)
    {
        var userChat = new UserChats(idUser);
        var request = new UserInfoRequest{ UserId = idUser.ToString() };
        
        UserChatsReponse reponse = await _userClient.GetUserChatsAsync(request)
            .ConfigureAwait(false);

        foreach (string chat in reponse.ChatsID) userChat.IdChats.Add(chat);

        return userChat;
    }
}


