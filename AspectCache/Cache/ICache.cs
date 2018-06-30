namespace AspectCache.Cache
{
    public interface ICache
    {
        bool Add(string key, object value, UseCacheAttribute cacheItemPolicy);
        object Get(string key);
    }
}
