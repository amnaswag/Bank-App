namespace BankApp.Services;

public class AccountService : IAccountService
{
    private readonly IStorageService _storageService;
    private const string AccountsKey = "BankApp_Accounts";

    public AccountService(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public IBankAccount CreateAccount(string name, AccountType accountType, string currency, decimal initalBalance) => throw new NotImplementedException("Använd CreateAccountAsync");
    public List<IBankAccount> GetAccounts() => throw new NotImplementedException("Använd GetAccountsAsync");

    public async Task<IBankAccount> CreateAccountAsync(string name, AccountType type, string currency, decimal initialBalance)
    {
        var newAccount = new BankAccount(name, type, currency, initialBalance);
        
        var accounts = await GetAccountsInternalAsync();
        accounts.Add(newAccount);

        await SaveAccountsAsync(accounts);
        
        return newAccount;
    }

    public async Task<List<IBankAccount>> GetAccountsAsync()
    {
        return (await GetAccountsInternalAsync()).Cast<IBankAccount>().ToList();
    }
    
    public async Task DeleteAccountAsync(Guid accountId)
    {
        var accounts = await GetAccountsInternalAsync();
        
        var accountToRemove = accounts.FirstOrDefault(a => a.Id == accountId);

        if (accountToRemove != null)
        {
            accounts.Remove(accountToRemove);
        }
        
        await SaveAccountsAsync(accounts);
    }

    private async Task<List<BankAccount>> GetAccountsInternalAsync()
    {
        return await _storageService.LoadAsync<List<BankAccount>>(AccountsKey) ?? new List<BankAccount>();
    }

    private async Task SaveAccountsAsync(List<BankAccount> accounts)
    {
        await _storageService.SaveAsync(AccountsKey, accounts);
    }
}