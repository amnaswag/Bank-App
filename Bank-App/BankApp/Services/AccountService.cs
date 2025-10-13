namespace BankApp.Services;

public class AccountService : IAccountService
{
    private readonly IStorageService _storageService;
    private const string AccountsKey = "BankApp_Accounts";
    private const string TransactionsKey = "BankApp_Transactions";

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

    public async Task<string> DepositAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0)
        {
            return "Beloppet måste vara större än noll.";
        }

        var accounts = await GetAccountsInternalAsync();
        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null)
        {
            return "Konto hittades inte.";
        }
        
        account.Deposit(amount);
        
        await CreateTransactionAsync(account.Id, amount, "Insättning", account.Balance);

        await SaveAccountsAsync(accounts);
        return string.Empty;
    }

    public async Task<string> WithdrawAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0)
        {
            return "Beloppet måste vara större än noll.";
        }

        var accounts = await GetAccountsInternalAsync();
        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null)
        {
            return "Konto hittades inte.";
        }
        
        if (account.Balance < amount)
        {
            return $"Övertrassering hindrad: Uttag på {amount:C2} är större än saldot {account.Balance:C2}.";
        }

        account.Withdrawn(amount);

        await CreateTransactionAsync(account.Id, amount * -1, "Uttag", account.Balance);
        
        await SaveAccountsAsync(accounts);
        return string.Empty;
    }

    public async Task<List<ITransaction>> GetTransactionsAsync(Guid accountId)
    {
        var allTransactions = await _storageService.LoadAsync<List<Transaction>>(TransactionsKey);
        
        return (allTransactions ?? new List<Transaction>())
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .Cast<ITransaction>()
            .ToList();
    }
    
    private async Task CreateTransactionAsync(Guid accountId, decimal amount, string type, decimal balanceAfter)
    {
        var newTransaction = new Transaction(amount, type, balanceAfter, accountId);
        
        var allTransactions = await _storageService.LoadAsync<List<Transaction>>(TransactionsKey) ?? new List<Transaction>();
        allTransactions.Add(newTransaction);
        
        await _storageService.SaveAsync(TransactionsKey, allTransactions);
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