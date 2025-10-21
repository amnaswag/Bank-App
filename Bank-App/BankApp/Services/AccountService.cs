namespace BankApp.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IStorageService _storageService; 
    private const string TransactionsKey = "BankApp_Transactions";
    private static readonly HashSet<Guid> _unlockedAccounts = new();

    public AccountService(IAccountRepository accountRepository, IStorageService storageService)
    {
        _accountRepository = accountRepository;
        _storageService = storageService;
    }

    public async Task<IBankAccount> CreateAccountAsync(string name, AccountType type, CurrencyType currency, decimal initialBalance, string? pinHash = null)
    {
        var newAccount = new BankAccount(name, type, currency, initialBalance, pinHash);
        
        await _accountRepository.AddAccountAsync(newAccount);
        
        if (!string.IsNullOrEmpty(pinHash))
        {
            _unlockedAccounts.Add(newAccount.Id);
        }
        
        return newAccount;
    }

    public async Task<List<IBankAccount>> GetAccountsAsync()
    {
        return (await _accountRepository.GetAllAccountsAsync()).Cast<IBankAccount>().ToList();
    }
    
    public async Task DeleteAccountAsync(Guid accountId)
    {
        await _accountRepository.DeleteAccountAsync(accountId);
        if (_unlockedAccounts.Contains(accountId))
        {
            _unlockedAccounts.Remove(accountId);
        }
    }

    public async Task<string> DepositAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0) return "Beloppet måste vara större än noll.";

        var accounts = await _accountRepository.GetAllAccountsAsync();
        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null) return "Konto hittades inte.";
        
        if (!string.IsNullOrEmpty(account.PinHash) && !_unlockedAccounts.Contains(accountId))
        {
             return "Konto är låst. Vänligen lås upp det först med PIN-kod för att göra en insättning/uttag.";
        }
        
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
        
        if (!string.IsNullOrEmpty(account.PinHash) && !_unlockedAccounts.Contains(accountId))
        {
             return "Konto är låst. Vänligen lås upp det först med PIN-kod för att göra ett uttag.";
        }
        
        if (account.Balance < amount) return $"Övertrassering hindrad: Uttag på {amount:N2} SEK är större än saldot {account.Balance:N2} SEK.";

        account.Withdrawn(amount);

        await CreateTransactionAsync(account.Id, TransactionType.Uttag, amount * -1, account.Balance);
        
        await _accountRepository.SaveAllAccountsAsync(accounts);
        return string.Empty;
    }

    public async Task<string> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        if (amount <= 0) return "Beloppet måste vara större än noll.";
        if (fromAccountId == toAccountId) return "Kan inte överföra pengar till samma konto.";

        var accounts = await _accountRepository.GetAllAccountsAsync();
        var fromAccount = accounts.FirstOrDefault(a => a.Id == fromAccountId);
        var toAccount = accounts.FirstOrDefault(a => a.Id == toAccountId);

        if (fromAccount == null || toAccount == null) return "Ett eller båda kontona hittades inte.";
        
        if (!string.IsNullOrEmpty(fromAccount.PinHash) && !_unlockedAccounts.Contains(fromAccountId))
        {
             return $"Avsändarkontot ({fromAccount.Name}) är låst. Vänligen lås upp det först med PIN-kod för att göra en överföring.";
        }
        
        if (fromAccount.Balance < amount)
        {
            return $"Övertrassering hindrad: Otillräckligt saldo på avsändarkontot ({fromAccount.Balance:N2} SEK).";
        }

        fromAccount.Withdrawn(amount);
        await CreateTransactionAsync(fromAccount.Id, TransactionType.Överföring, amount * -1, fromAccount.Balance, toAccount.Id);

        toAccount.Deposit(amount);
        await CreateTransactionAsync(toAccount.Id, TransactionType.Överföring, amount, toAccount.Balance, fromAccount.Id);

        await _accountRepository.SaveAllAccountsAsync(accounts);

        return string.Empty;
    }
    
    public async Task<string> ApplyInterestAsync(decimal interestRate)
    {
        if (interestRate <= 0) return "Räntan måste vara positiv.";
        
        var accounts = await _accountRepository.GetAllAccountsAsync();
        int affectedAccounts = 0;
        
        foreach (var account in accounts.Where(a => a.AccountType == AccountType.Sparkonto))
        {
            if (!string.IsNullOrEmpty(account.PinHash) && !_unlockedAccounts.Contains(account.Id))
            {
                 continue; 
            }
            
            decimal interestAmount = account.Balance * interestRate;
            
            if (interestAmount > 0)
            {
                account.Deposit(interestAmount);
                await CreateTransactionAsync(account.Id, TransactionType.Insättning, interestAmount, account.Balance);
                affectedAccounts++;
            }
        }
        
        await _accountRepository.SaveAllAccountsAsync(accounts);
        
        if (affectedAccounts == 0)
        {
            return "Inga upplåsta sparkonton hittades att applicera ränta på.";
        }
        
        return string.Empty;
    }

    public void Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        TransferAsync(fromAccountId, toAccountId, amount).GetAwaiter().GetResult();
    }

    public async Task<List<Transaction>> GetTransactionsAsync(Guid accountId)
    {
        var allTransactions = await _storageService.LoadAsync<List<Transaction>>(TransactionsKey);
        
        return (allTransactions ?? new List<Transaction>())
            .Where(t => t.AccountId == accountId)
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
            _unlockedAccounts.Add(accountId);
            return true;
        }
        return false;
    }
    
    public bool IsAccountUnlocked(Guid accountId)
    {
        return _unlockedAccounts.Contains(accountId);
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