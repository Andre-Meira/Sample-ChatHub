using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Sample.ChatHub.Server.Infrastructure;

public interface ICacheService
{
    Task<UserChats?> GetUserChats(Guid idUser);        
}


internal class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<UserChats?> GetUserChats(Guid idUser)
    {
        string? json = await _distributedCache.GetStringAsync(idUser.ToString());

        if (json is null) return null;

        return JsonConvert.DeserializeObject<UserChats>(json);
    }
}
