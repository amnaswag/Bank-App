namespace BankApp.Domain;

public class BankAccount : IBankAccount
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public AccountType AccountType { get; private set; }
    public string Currency { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public string? PinHash { get; private set; }

    public BankAccount(string name, AccountType accountType, string currency, decimal initalBalance, string? pinHash = null)
    {
        Name = name;
        AccountType = accountType;
        Currency = currency;
        Balance = initalBalance;
        LastUpdated = DateTime.Now;
        PinHash = pinHash;
    }
    
    [JsonConstructor]
    public BankAccount(Guid id, string name, AccountType accountType, string currency, decimal balance, DateTime lastUpdated, string? pinHash)
    {
        Id = id;
        Name = name;
        AccountType = accountType;
        Currency = currency;
        Balance = balance;
        LastUpdated = lastUpdated;
        PinHash = pinHash;
    }

    public void Withdrawn(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Beloppet får inte vara negativt.", nameof(amount));
        if (Balance < amount) throw new InvalidOperationException("Otillräckligt saldo.");

        Balance -= amount;
        LastUpdated = DateTime.Now;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Beloppet får inte vara negativt.", nameof(amount));

        Balance += amount;
        LastUpdated = DateTime.Now;
    }
}