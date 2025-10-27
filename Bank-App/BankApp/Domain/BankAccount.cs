namespace BankApp.Domain;

/// <summary>
/// Represents a single bank account entity in the domain.
/// Implements IBankAccount for contract assurance.
/// </summary>
public class BankAccount : IBankAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public AccountType AccountType { get; set; }
    public CurrencyType Currency { get; set; }
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; }
    public string? PinHash { get; set; }

    /// <summary>
    /// Constructor used when creating a new bank account.
    /// </summary>
    public BankAccount(string name, AccountType accountType, CurrencyType currency, decimal initalBalance, string? pinHash = null)
    {
        Name = name;
        AccountType = accountType;
        Currency = currency;
        Balance = initalBalance;
        LastUpdated = DateTime.Now;
        PinHash = pinHash;
    }
    
    /// <summary>
    /// JsonConstructor used for deserializing data saved in LocalStorage.
    /// </summary>
    [JsonConstructor]
    public BankAccount(Guid id, string name, AccountType accountType, CurrencyType currency, decimal balance, DateTime lastUpdated, string? pinHash)
    {
        Id = id;
        Name = name;
        AccountType = accountType;
        Currency = currency;
        Balance = balance;
        LastUpdated = lastUpdated;
        PinHash = pinHash;
    }

    /// <summary>
    /// Decreases the account balance by the specified amount.
    /// Throws exceptions for invalid amounts or overdrafts.
    /// </summary>
    public void Withdrawn(decimal amount)
    {
        if (amount <= 0) 
            throw new ArgumentException("Beloppet får inte vara negativt.", nameof(amount));
        if (Balance < amount) 
            throw new InvalidOperationException("Otillräckligt saldo.");

        Balance -= amount;
        LastUpdated = DateTime.Now;
    }

    /// <summary>
    /// Increases the account balance by the specified amount.
    /// Throws exception for invalid amounts.
    /// </summary>
    public void Deposit(decimal amount)
    {
        if (amount <= 0) 
            throw new ArgumentException("Beloppet får inte vara negativt.", nameof(amount));

        Balance += amount;
        LastUpdated = DateTime.Now;
    }
}