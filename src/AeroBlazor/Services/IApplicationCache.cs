namespace AeroBlazor.Services;

public interface IApplicationCache<TKey, TValue>
{
    Task<List<TValue>> ReadCollectionAsync(Func<Task<IDictionary<string, TValue>>> readAction);
    Task<TValue> ReadThroughAsync(TKey cacheKey, Func<TKey, Task<TValue>> readAction);
    Task WriteBehindAsync(TKey cacheKey, TValue updatedValue, Func<TKey, TValue, Task> persistAction);
    Task FullDeleteAsync(TKey cacheKey, Func<TKey, Task> deleteAction);
}