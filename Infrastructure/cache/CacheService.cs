using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key,Func<Task<T>> factory,int expirationInMinutes)
    {
        if (_cache.TryGetValue(key, out T cachedValue))
        {
            return cachedValue;
        }

        var value = await factory();

        _cache.Set(key, value, TimeSpan.FromMinutes(expirationInMinutes));
        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}