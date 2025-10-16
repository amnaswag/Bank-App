namespace BankApp.Interfaces;

public interface IAccountRepository
{
    Task<List<BankAccount>> GetAllAccountsAsync();
    Task AddAccountAsync(BankAccount account);
    Task UpdateAccountAsync(BankAccount account);
    Task DeleteAccountAsync(Guid accountId);
    Task SaveAllAccountsAsync(List<BankAccount> accounts);
}