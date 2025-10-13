namespace BankApp.Interfaces;

public interface IStorageService
{
    Task SaveAsync<T>(string key, T data);
    Task<T> LoadAsync<T>(string key);
}