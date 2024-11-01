namespace AeroBlazor.Extensions;

public static class DictionaryExtensionMethods
{
    public static void Upsert<T>(this IDictionary<string, T> dictionary, string key, T value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
        
    public static T GetValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue = default(T))
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            if (value is T castedValue) {
                return castedValue;
            } 
            try {
                return (T)Convert.ChangeType(value, typeof(T));
            } 
            catch (InvalidCastException)
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }
}