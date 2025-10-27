namespace BankApp.Services;

/// <summary>
/// Implements the IAccountRepository contract for managing bank account persistence.
/// Relies on IStorageService for actual data interaction.
/// </summary>
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

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    public async Task AddAccountAsync(BankAccount account)
    {
        var accounts = await GetAllAccountsAsync();
        accounts.Add(account);
        await SaveAllAccountsAsync(accounts);
    }

    /// <summary>
    /// Updates an existing account by replacing the old object with the new one based on ID.
    /// </summary>
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

    /// <summary>
    /// Deletes an account from the list by ID and saves the updated list.
    /// </summary>
    public async Task DeleteAccountAsync(Guid accountId)
    {
        var accounts = await GetAllAccountsAsync();
        accounts.RemoveAll(a => a.Id == accountId);
        await SaveAllAccountsAsync(accounts);
    }

    /// <summary>
    /// Overwrites the entire list of accounts in persistent storage.
    /// </summary>
    public async Task SaveAllAccountsAsync(List<BankAccount> accounts)
    {
        await _storageService.SaveAsync(AccountsKey, accounts);
    }
}