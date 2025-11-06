namespace BankApp.Services;

/// <summary>
/// Implements the core business logic for all account operations.
/// Handles balance changes, validation, and transaction recording.
/// </summary>
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IStorageService _storageService; 
    private const string TransactionsKey = "BankApp_Transactions";
    private static readonly HashSet<Guid> _unlockedAccounts = new();

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    public AccountService(IAccountRepository accountRepository, IStorageService storageService)
    {
        _accountRepository = accountRepository;
        _storageService = storageService;
    }

    /// <summary>
    /// Creates a new BankAccount, saves it to the repository, and auto-unlocks it if a PIN was set.
    /// </summary>
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

    /// <summary>
    /// Processes a deposit operation, adding funds and recording a transaction.
    /// Includes validation for amount and PIN-lock check.
    /// </summary>
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
        
        await CreateTransactionAsync(account.Id, TransactionType.Deposit, amount, account.Balance);

        await _accountRepository.SaveAllAccountsAsync(accounts);
        return string.Empty;
    }

    /// <summary>
    /// Processes a withdrawal operation, deducting funds and recording a transaction.
    /// Includes validation for amount, overdraft, and PIN-lock check.
    /// </summary>
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

        await CreateTransactionAsync(account.Id, TransactionType.Withdrawal, amount * -1, account.Balance);
        
        await _accountRepository.SaveAllAccountsAsync(accounts);
        return string.Empty;
    }

    /// <summary>
    /// Processes an account transfer, handling checks, fund deduction/addition, and recording two transactions.
    /// Includes PIN-lock check on the sender's account.
    /// </summary>
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
        await CreateTransactionAsync(fromAccount.Id, TransactionType.Transfer, amount * -1, fromAccount.Balance, toAccount.Id);

        toAccount.Deposit(amount);
        await CreateTransactionAsync(toAccount.Id, TransactionType.Transfer, amount, toAccount.Balance, fromAccount.Id);

        await _accountRepository.SaveAllAccountsAsync(accounts);

        return string.Empty;
    }
    
    /// <summary>
    /// Applies the specified interest rate to all unlocked Savings Accounts 
    /// Skips PIN-locked accounts that haven't been unlocked this session.
    /// </summary>
    public async Task<string> ApplyInterestAsync(decimal interestRate)
    {
        if (interestRate <= 0) return "Räntan måste vara positiv.";
        
        var accounts = await _accountRepository.GetAllAccountsAsync();
        int affectedAccounts = 0;
        
        foreach (var account in accounts.Where(a => a.AccountType == AccountType.SavingsAccount))
        {
            if (!string.IsNullOrEmpty(account.PinHash) && !_unlockedAccounts.Contains(account.Id))
            {
                 continue; 
            }
            
            decimal interestAmount = account.Balance * interestRate;
            
            if (interestAmount > 0)
            {
                account.Deposit(interestAmount);
                await CreateTransactionAsync(account.Id, TransactionType.Deposit, interestAmount, account.Balance);
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

    /// <summary>
    /// Synchronous wrapper for TransferAsync.
    /// </summary>
    public void Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        TransferAsync(fromAccountId, toAccountId, amount).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Retrieves all transactions for a single specified account.
    /// </summary>
    public async Task<List<Transaction>> GetTransactionsAsync(Guid accountId)
    {
        var allTransactions = await _storageService.LoadAsync<List<Transaction>>(TransactionsKey);
        
        return (allTransactions ?? new List<Transaction>())
            .Where(t => t.AccountId == accountId)
            .ToList();
    }
    
    /// <summary>
    /// Exports all data (accounts and transactions) to a formatted JSON string (VG-Tillägg).
    /// </summary>
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

    /// <summary>
    /// Imports data from a JSON string, overwriting current accounts and transactions (VG-Tillägg).
    /// Includes validation for JSON format and data presence.
    /// </summary>
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
    
    /// <summary>
    /// Attempts to unlock a PIN-protected account for the current session.
    /// </summary>
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
    
    /// <summary>
    /// Checks the in-memory set to see if an account is currently unlocked.
    /// </summary>
    public bool IsAccountUnlocked(Guid accountId)
    {
        return _unlockedAccounts.Contains(accountId);
    }

    /// <summary>
    /// Private model for serializing and deserializing export/import data structure.
    /// </summary>
    private class ExportModel  
    {
        public List<BankAccount>? Accounts { get; set; }
        public List<Transaction>? Transactions { get; set; }
    }
    
    /// <summary>
    /// Helper method to create a new transaction record and save the updated list.
    /// </summary>
    private async Task CreateTransactionAsync(Guid accountId, TransactionType type, decimal amount, decimal balanceAfter, Guid? toAccountId = null)
    {
        Transaction newTransaction;
        
        if (type == TransactionType.Transfer && toAccountId.HasValue)
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