namespace BankApp.Services;
/// <summary>
/// Implements IStorageService using JavaScript Interop to utilize the browser's LocalStorage.
/// </summary>
public class StorageService : IStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public StorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Serializes and saves data to LocalStorage.
    /// </summary>
    public async Task SaveAsync<T>(string key, T data)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Loads and deserializes data from LocalStorage.
    /// </summary>
    public async Task<T> LoadAsync<T>(string key)
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }
}