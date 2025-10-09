namespace BankApp.Domain;

public class BankAccount : IBankAccount
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public AccountType AccountType { get; private set; }
    public string Currency { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime LastUpdated { get; private set; }

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
        throw new NotImplementedException();
    }

    public void Deposit(decimal amount)
    {
        throw new NotImplementedException();
    }
}

