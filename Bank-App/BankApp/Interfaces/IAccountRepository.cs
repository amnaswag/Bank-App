namespace BankApp.Interfaces;

/// <summary>
/// Defines the contract for persistence operations on Bank Accounts.
/// </summary>
public interface IAccountRepository
{
    // Handles loading and saving data to the persistent store (LocalStorage).
    Task<List<BankAccount>> GetAllAccountsAsync(); 
    Task AddAccountAsync(BankAccount account);  
    Task UpdateAccountAsync(BankAccount account);  
    Task DeleteAccountAsync(Guid accountId);  
    Task SaveAllAccountsAsync(List<BankAccount> accounts);  
}