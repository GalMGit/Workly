using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Modules.Identity.Application.Abstractions.Cache;

namespace Modules.Identity.Infrastructure.Cache;

public class RedisCacheService(
    IDistributedCache cache) 
    : ICacheService
{
    private readonly DistributedCacheEntryOptions _defaultOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
    };

    public async Task<bool> ExistsAsync(
        string key, 
        CancellationToken ct = default)
    {
        var data = await cache.GetStringAsync(key, ct);
        return !string.IsNullOrEmpty(data);
    }

    public async Task<T?> GetAsync<T>(
        string key, 
        CancellationToken ct = default)
    {
        var data = await cache.GetStringAsync(key, ct);

        return string.IsNullOrEmpty(data)
            ? default 
            : JsonSerializer.Deserialize<T>(data);
    }

    public async Task RemoveAsync(
        string key, 
        CancellationToken ct = default)
        => await cache.RemoveAsync(key, ct);
    

    public async Task SetAsync<T>(
        string key, 
        T value, 
        TimeSpan? expiry = null, 
        CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = 
                expiry ?? _defaultOptions.AbsoluteExpirationRelativeToNow
        };

        var jsonData = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(
            key, 
            jsonData, 
            options, ct);
    }
}