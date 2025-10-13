namespace BankApp.Domain;

public class BankAccount : IBankAccount
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public AccountType AccountType { get; private set; }
    public string Currency { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime LastUpdated { get; private set; }

    [JsonConstructor]
    public BankAccount(Guid id, string name, AccountType accountType, string currency, decimal balance, DateTime lastUpdated)
    {
        Id = id; 
        Name = name;
        AccountType = accountType;
        Currency = currency;
        Balance = balance;
        LastUpdated = lastUpdated;
    }

    public BankAccount(string name, AccountType accountType, string currency, decimal initalBalance)
    {
        Name = name;
        AccountType = accountType;
        Currency = currency;
        Balance = initalBalance;
        LastUpdated = DateTime.Now;
    }

    public void Withdrawn(decimal amount)
    {
        if (amount > 0 && Balance >= amount)
        {
            Balance -= amount;
            LastUpdated = DateTime.Now;
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            LastUpdated = DateTime.Now;
        }
    }
}