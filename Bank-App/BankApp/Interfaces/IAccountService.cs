namespace BankApp.Interfaces;

public interface IAccountService
{
    Task<IBankAccount> CreateAccountAsync(string name, AccountType accountType, CurrencyType currency, decimal initalBalance, string? pinHash = null);
    Task<List<IBankAccount>> GetAccountsAsync();
    Task DeleteAccountAsync(Guid accountId);
    Task<string> DepositAsync(Guid accountId, decimal amount);
    Task<string> WithdrawAsync(Guid accountId, decimal amount);
    Task<string> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
    void Transfer(Guid fromAccountId, Guid toAccountId, decimal amount);

    Task<List<Transaction>> GetTransactionsAsync(Guid accountId); 

    Task<bool> UnlockAccountAsync(Guid accountId, string pin);
    bool IsAccountUnlocked(Guid accountId);
    Task<string> ExportDataToJsonAsync();
    Task<string> ImportDataFromJsonAsync(string jsonData);
    Task<string> ApplyInterestAsync(decimal interestRate);
}