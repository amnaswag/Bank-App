namespace BankApp.Domain;

/// <summary>
/// Represents a single transaction record associated with a bank account.
/// </summary>
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public Guid AccountId { get; set; }
    public Guid? ToAccountId { get; set; }
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// Constructor for simple transactions (Deposit or Withdrawal).
    /// </summary>
    public Transaction(Guid accountId, TransactionType transactionType, decimal amount, decimal balanceAfter)
    {
        Date = DateTime.Now;
        Amount = amount;
        BalanceAfter = balanceAfter;
        AccountId = accountId;
        TransactionType = transactionType;
    }

    /// <summary>
    /// Constructor used specifically for Transfer transactions.
    /// Sets TransactionType to Transfer automatically.
    /// </summary>
    public Transaction(Guid fromAccountId, Guid toAccountId, decimal amount, decimal balanceAfter)
    {
        Date = DateTime.Now;
        Amount = amount;
        BalanceAfter = balanceAfter;
        AccountId = fromAccountId;
        ToAccountId = toAccountId;
        TransactionType = TransactionType.Transfer;
    }
    
    /// <summary>
    /// JsonConstructor used for deserialization from persistent storage.
    /// </summary>
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