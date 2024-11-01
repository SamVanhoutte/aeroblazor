namespace AeroBlazor.Caching;

internal class CachedItem<T>
{
    public DateTime CacheEntry { get; set; }
    public DateTime CacheExpiration { get; set; }
    public T Value { get; set; }
    public bool IsValid => CacheExpiration > DateTime.UtcNow && Value != null;

    public CachedItem(T item, TimeSpan timeToCache, DateTime cacheEntry = default)
    {
        Value = item;
        CacheExpiration = DateTime.UtcNow.Add(timeToCache);
        CacheEntry = cacheEntry == default ? DateTime.UtcNow : cacheEntry;
    }
}