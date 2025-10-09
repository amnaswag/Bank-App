namespace BankApp.Interfaces
{
    public interface IAccountService
    {
        IBankAccount CreateBankAccount(string name, string currency, decimal initalBalance);
        List<IBankAccount> GetAccounts();
    }
}
