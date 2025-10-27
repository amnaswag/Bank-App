namespace BankApp.Interfaces;

/// <summary>
/// Interface defining the core contract for a Bank Account.
/// </summary>
public interface IBankAccount
{
    Guid Id { get; set; }
    string Name { get; set; }
    AccountType AccountType { get; set; }
    CurrencyType Currency { get; set; }
    decimal Balance { get; set; }
    DateTime LastUpdated { get; set; }
    
    string? PinHash { get; set; }

    
    // Adds an amount to the balance (deposit logic).
    void Withdrawn(decimal amount);
    void Deposit(decimal amount);
}