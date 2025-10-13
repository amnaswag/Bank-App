namespace BankApp.Interfaces
{
    public interface IAccountService
    {
        Task<IBankAccount> CreateAccountAsync(string name, AccountType accountType, string currency, decimal initalBalance);
        Task<List<IBankAccount>> GetAccountsAsync();
        
        Task DeleteAccountAsync(Guid accountId);
    }
}