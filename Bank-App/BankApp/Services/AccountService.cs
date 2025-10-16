namespace BankApp.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IStorageService _storageService; 
    private const string TransactionsKey = "BankApp_Transactions";

    public AccountService(IAccountRepository accountRepository, IStorageService storageService)
    {
        _accountRepository = accountRepository;
        _storageService = storageService;
    }

    public async Task<IBankAccount> CreateAccountAsync(string name, AccountType type, string currency, decimal initialBalance, string? pinHash = null)
    {
        var newAccount = new BankAccount(name, type, currency, initialBalance, pinHash);
        
        await _accountRepository.AddAccountAsync(newAccount);
        
        return newAccount;
    }

    public async Task<List<IBankAccount>> GetAccountsAsync()
    {
        return (await _accountRepository.GetAllAccountsAsync()).Cast<IBankAccount>().ToList();
    }
    
    public async Task DeleteAccountAsync(Guid accountId)
    {
        await _accountRepository.DeleteAccountAsync(accountId);
    }

    public async Task<string> DepositAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0) return "Beloppet måste vara större än noll.";

        var accounts = await _accountRepository.GetAllAccountsAsync();
        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null) return "Konto hittades inte.";
        
        account.Deposit(amount);
        
        await CreateTransactionAsync(account.Id, TransactionType.Insättning, amount, account.Balance);

        await _accountRepository.SaveAllAccountsAsync(accounts);
        return string.Empty;
    }

    public async Task<string> WithdrawAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0) return "Beloppet måste vara större än noll.";

        var accounts = await _accountRepository.GetAllAccountsAsync();
        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null) return "Konto hittades inte.";
        
        if (account.Balance < amount) return $"Övertrassering hindrad: Uttag på {amount:C2} är större än saldot {account.Balance:C2}.";

        account.Withdrawn(amount);

        await CreateTransactionAsync(account.Id, TransactionType.Uttag, amount * -1, account.Balance);
        
        await _accountRepository.SaveAllAccountsAsync(accounts);
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
    
    public async Task<string> ExportDataToJsonAsync()
    {
        var allAccounts = await _accountRepository.GetAllAccountsAsync();
        var allTransactions = await _storageService.LoadAsync<List<Transaction>>(TransactionsKey) ?? new List<Transaction>();

        var exportModel = new ExportModel
        {
            Accounts = allAccounts,
            Transactions = allTransactions
        };

        return JsonSerializer.Serialize(exportModel, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task<string> ImportDataFromJsonAsync(string jsonData)
    {
        try
        {
            var importModel = JsonSerializer.Deserialize<ExportModel>(jsonData);

            if (importModel?.Accounts == null)
            {
                return "Fel: Kunde inte hitta kontodata i filen.";
            }

            await _accountRepository.SaveAllAccountsAsync(importModel.Accounts);
            await _storageService.SaveAsync(TransactionsKey, importModel.Transactions ?? new List<Transaction>());

            return string.Empty;
        }
        catch (JsonException ex)
        {
            return $"Fel vid tolkning av JSON-filen: {ex.Message}";
        }
        catch (Exception)
        {
            return "Ett okänt fel inträffade under importen.";
        }
    }
    
    public async Task<bool> UnlockAccountAsync(Guid accountId, string pin)
    {
        var accounts = await _accountRepository.GetAllAccountsAsync();
        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account?.PinHash == pin)
        {
            return true;
        }
        return false;
    }

    private class ExportModel
    {
        public List<BankAccount>? Accounts { get; set; }
        public List<Transaction>? Transactions { get; set; }
    }
    
    private async Task CreateTransactionAsync(Guid accountId, TransactionType type, decimal amount, decimal balanceAfter, Guid? toAccountId = null)
    {
        Transaction newTransaction;
        
        if (type == TransactionType.Överföring && toAccountId.HasValue)
        {
            newTransaction = new Transaction(accountId, toAccountId.Value, amount, balanceAfter);
        }
        else
        {
            newTransaction = new Transaction(accountId, type, amount, balanceAfter);
        }
        
        var allTransactions = await _storageService.LoadAsync<List<Transaction>>(TransactionsKey) ?? new List<Transaction>();
        allTransactions.Add(newTransaction);
        
        await _storageService.SaveAsync(TransactionsKey, allTransactions);
    }
}