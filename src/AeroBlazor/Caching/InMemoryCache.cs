using AeroBlazor.Extensions;

namespace AeroBlazor.Caching;

public class InMemoryCache<TKey, TValue> : IApplicationCache<TKey, TValue>
{
    private IDictionary<string, CachedItem<TValue>> memoryCache = new Dictionary<string, CachedItem<TValue>>();
    private TimeSpan defaultCacheDuration;
    private DateTime? lastReset;

    public InMemoryCache(TimeSpan cacheDuration)
    {
        defaultCacheDuration = cacheDuration;
    }

    public async Task<List<TValue>> ReadCollectionAsync(Func<Task<IDictionary<string, TValue>>> readAction)
    {
        if (lastReset == null || memoryCache?.Count == 0 ||
            lastReset.Value.Add(defaultCacheDuration) <= DateTime.UtcNow)
        {
            var newEntries = await readAction();
            lock (memoryCache)
            {

                memoryCache = new Dictionary<string, CachedItem<TValue>>();
                foreach (var newValue in newEntries)
                {
                    var newCacheItem = new CachedItem<TValue>(newValue.Value, defaultCacheDuration);
                    memoryCache.Upsert(newValue.Key.ToString(), newCacheItem);
                }
            }

            lastReset = DateTime.UtcNow;
        }

        return memoryCache.Select(m => m.Value.Value).ToList();
    }

    public async Task<TValue> ReadThroughAsync(TKey cacheKey, Func<TKey, Task<TValue>> readAction)
    {
        lock (memoryCache)
        {
            if (memoryCache.TryGetValue(cacheKey.ToString(), out var cacheEntry))
            {
                if (cacheEntry.IsValid)
                {
                    // Value found that is not expired, so returning that
                    return cacheEntry.Value;
                }
            }
        }

        var newEntry = await readAction(cacheKey);
        lock (memoryCache)
        {
            if (newEntry != null)
            {
                var newCacheItem = new CachedItem<TValue>(newEntry, defaultCacheDuration);
                memoryCache.Upsert(cacheKey.ToString(), newCacheItem);
            }

            return newEntry;
        }
    }


    public async Task WriteBehindAsync(TKey cacheKey, TValue updatedValue, Func<TKey, TValue, Task> persistAction)
    {
        // Execute persistence first
        await persistAction(cacheKey, updatedValue);
        lock (memoryCache)
        {
            memoryCache.Upsert(cacheKey.ToString(), new CachedItem<TValue>(updatedValue, defaultCacheDuration));
        }
    }


    public async Task FullDeleteAsync(TKey cacheKey, Func<TKey, Task> deleteAction)
    {
        if (memoryCache.ContainsKey(cacheKey.ToString()))
        {
            memoryCache.Remove(cacheKey.ToString());
        }

        await deleteAction(cacheKey);
    }
}