using Domain.Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure;

public class CacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private readonly IDatabase database = connectionMultiplexer.GetDatabase();
    public string GetData(string cacheKey)
    {
        var data = database.StringGet(cacheKey);
        if (!string.IsNullOrEmpty(data))
        {
            return data!;
        }
        return null!;
    }
    public bool SetData(string key, object value, int timeToLive)
    {
        // serialize the data
        var serializedData = JsonSerializer.Serialize(value);
        var TTL = TimeSpan.FromSeconds(timeToLive);
        // set the data
        return database.StringSet(key, serializedData, TTL);
    }

    public bool RemoveData(string key)
    {
        return database.KeyDelete(key);
    }
}
