namespace AeroBlazor.Services;

public interface IApplicationStorageProvider
{
    Task DeleteFromStorageAsync(string key, bool secret = false);
    Task<T> ReadFromStorageAsync<T>(string key, bool secret = false, T defaultValue = default);
    Task SaveToStorageAsync<T>(string key, T value, bool secret = false);
}