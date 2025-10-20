namespace BankApp.Domain;

public class Transaction
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Date { get; }
    public decimal Amount { get; }
    public decimal BalanceAfter { get; }
    public Guid AccountId { get; }
    public Guid? ToAccountId { get; }
    public TransactionType TransactionType { get; }

    public Transaction(Guid accountId, TransactionType transactionType, decimal amount, decimal balanceAfter)
    {
        Date = DateTime.Now;
        Amount = amount;
        BalanceAfter = balanceAfter;
        AccountId = accountId;
        TransactionType = transactionType;
    }

    public Transaction(Guid fromAccountId, Guid toAccountId, decimal amount, decimal balanceAfter)
    {
        Date = DateTime.Now;
        Amount = amount;
        BalanceAfter = balanceAfter;
        AccountId = fromAccountId;
        ToAccountId = toAccountId;
        TransactionType = TransactionType.Överföring;
    }
    
    [JsonConstructor]
    public Transaction(Guid id, DateTime date, decimal amount, decimal balanceAfter, Guid accountId, TransactionType transactionType, Guid? toAccountId)
    {
        Id = id;
        Date = date;
        Amount = amount;
        BalanceAfter = balanceAfter;
        AccountId = accountId;
        TransactionType = transactionType;
        ToAccountId = toAccountId;
    }
}