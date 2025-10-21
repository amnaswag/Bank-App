namespace BankApp.Interfaces;

public interface IBankAccount
{
    Guid Id { get; set; }
    string Name { get; set; }
    AccountType AccountType { get; set; }
    CurrencyType Currency { get; set; }
    decimal Balance { get; set; }
    DateTime LastUpdated { get; set; }
    
    string? PinHash { get; set; }

    void Withdrawn(decimal amount);
    void Deposit(decimal amount);
}