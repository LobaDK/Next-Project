namespace API.Services;

/// <summary>
/// Simple cache service for storing temporary data like pagination sessions
/// </summary>
public class CacheService
{
    private readonly Dictionary<string, object> _cache = new();
    
    public void Set<T>(string key, T value)
    {
        _cache[key] = value!;
    }
    
    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default;
    }
    
    public bool TryGetValue<T>(string key, out T? value)
    {
        if (_cache.TryGetValue(key, out var obj) && obj is T)
        {
            value = (T)obj;
            return true;
        }
        value = default;
        return false;
    }
    
    public void Remove(string key)
    {
        _cache.Remove(key);
    }
    
    public void Clear()
    {
        _cache.Clear();
    }
}