namespace BankApp.Interfaces;

/// <summary>
/// Defines the contract for local, persistent storage operations (JS Interop / LocalStorage).
/// </summary>
public interface IStorageService
{
   // Saves data
   Task SaveAsync<T>(string key, T data); 
   // Loads data 
    Task<T> LoadAsync<T>(string key); 
}