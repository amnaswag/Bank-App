namespace BankApp.Domain;

public class Transaction : ITransaction
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Date { get; }
    public decimal Amount { get; }
    public string Type { get; }
    public decimal BalanceAfter { get; }
    public Guid AccountId { get; }
    public Guid? TransferTargetId { get; }

    public Transaction(decimal amount, string type, decimal balanceAfter, Guid accountId, Guid? transferTargetId = null)
    {
        Date = DateTime.Now;
        Amount = amount;
        Type = type;
        BalanceAfter = balanceAfter;
        AccountId = accountId;
        TransferTargetId = transferTargetId;
    }
}