namespace BankApp.Domain;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public Guid AccountId { get; set; }
    public Guid? ToAccountId { get; set; }
    public TransactionType TransactionType { get; set; }

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