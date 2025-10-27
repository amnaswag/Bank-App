namespace BankApp.Interfaces;

/// <summary>
/// Defines the contract for all core business logic related to accounts and transactions.
/// </summary>
public interface IAccountService
{
    Task<IBankAccount> CreateAccountAsync(string name, AccountType accountType, CurrencyType currency, decimal initalBalance, string? pinHash = null);
    Task<List<IBankAccount>> GetAccountsAsync();
    Task DeleteAccountAsync(Guid accountId);
    Task<string> DepositAsync(Guid accountId, decimal amount);
    Task<string> WithdrawAsync(Guid accountId, decimal amount);
    Task<string> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
    void Transfer(Guid fromAccountId, Guid toAccountId, decimal amount);

    // Handles the deposit logic and records a transaction.
    Task<List<Transaction>> GetTransactionsAsync(Guid accountId); 

    Task<bool> UnlockAccountAsync(Guid accountId, string pin);
    bool IsAccountUnlocked(Guid accountId);
    Task<string> ExportDataToJsonAsync();
    
    // Imports account and transaction data from a JSON string, replacing existing data.
    Task<string> ImportDataFromJsonAsync(string jsonData);  
    
    // Applies a specified interest rate to all eligible (unlocked) Savings Accounts.
    Task<string> ApplyInterestAsync(decimal interestRate);
}