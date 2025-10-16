namespace BankApp.Services;

public class AccountRepository : IAccountRepository
{
    private readonly IStorageService _storageService;
    private const string AccountsKey = "BankApp_Accounts";

    public AccountRepository(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<List<BankAccount>> GetAllAccountsAsync()
    {
        return await _storageService.LoadAsync<List<BankAccount>>(AccountsKey) ?? new List<BankAccount>();
    }

    public async Task AddAccountAsync(BankAccount account)
    {
        var accounts = await GetAllAccountsAsync();
        accounts.Add(account);
        await SaveAllAccountsAsync(accounts);
    }

    public async Task UpdateAccountAsync(BankAccount account)
    {
        var accounts = await GetAllAccountsAsync();
        var existing = accounts.FirstOrDefault(a => a.Id == account.Id);

        if (existing != null)
        {
            accounts.Remove(existing);
            accounts.Add(account);
            await SaveAllAccountsAsync(accounts);
        }
    }

    public async Task DeleteAccountAsync(Guid accountId)
    {
        var accounts = await GetAllAccountsAsync();
        accounts.RemoveAll(a => a.Id == accountId);
        await SaveAllAccountsAsync(accounts);
    }

    public async Task SaveAllAccountsAsync(List<BankAccount> accounts)
    {
        await _storageService.SaveAsync(AccountsKey, accounts);
    }
}