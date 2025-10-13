namespace BankApp.Interfaces
{
    public interface IAccountService
    {
        Task<IBankAccount> CreateAccountAsync(string name, AccountType accountType, string currency, decimal initalBalance);
        Task<List<IBankAccount>> GetAccountsAsync();
        
        Task DeleteAccountAsync(Guid accountId);

        Task<string> DepositAsync(Guid accountId, decimal amount);
        Task<string> WithdrawAsync(Guid accountId, decimal amount);
        Task<List<ITransaction>> GetTransactionsAsync(Guid accountId);
    }
}