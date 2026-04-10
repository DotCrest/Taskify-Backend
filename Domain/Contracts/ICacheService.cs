namespace Domain.Contracts;

public interface ICacheService
{
    string GetData(string cacheKey);
    bool SetData(string key, object value, int TTL);
    bool RemoveData(string key);
}
