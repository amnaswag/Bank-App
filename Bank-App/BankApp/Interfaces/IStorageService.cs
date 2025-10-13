namespace BankApp.Interfaces;

public interface IStorageService
{
    Task SaveAsync<T>(string key, T data); // Sparar 
    Task<T> LoadAsync<T>(string key); // Hämta
}