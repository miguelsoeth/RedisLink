using System.Text.Json;
using StackExchange.Redis;

namespace RedisLink.Services;

public interface ICacheService
{
    T GetData<T>(string key);
    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    object RemoveData(string key);
}

public class CacheService : ICacheService
{
    private IDatabase _cacheDb;

    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect("host.docker.internal:6379");
        _cacheDb = redis.GetDatabase();
    }
    
    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);
        if (!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>(value);
        
        return default;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expireTime = expirationTime.DateTime.Subtract(DateTime.Now);
        return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expireTime);
        
    }

    public object RemoveData(string key)
    {
        var _exist = _cacheDb.KeyExists(key);

        if (_exist)
            return _cacheDb.KeyDelete(key);
        
        return false;
    }
}