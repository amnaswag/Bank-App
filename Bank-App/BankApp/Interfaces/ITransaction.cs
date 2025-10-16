namespace BankApp.Interfaces;

public interface ITransaction
{
    Guid Id { get; }
    DateTime Date { get; }
    decimal Amount { get; }
    decimal BalanceAfter { get; }
    
    Guid AccountId { get; }
    Guid? ToAccountId { get; }
    
    TransactionType TransactionType { get; }
}