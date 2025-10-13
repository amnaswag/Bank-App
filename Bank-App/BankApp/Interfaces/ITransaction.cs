namespace BankApp.Interfaces;

public interface ITransaction
{
    Guid Id { get; }
    DateTime Date { get; }
    decimal Amount { get; }
    string Type { get; } // Insättning, Uttag, Överföring
    decimal BalanceAfter { get; }
    Guid AccountId { get; }
    Guid? TransferTargetId { get; }
}