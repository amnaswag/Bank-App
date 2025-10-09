namespace BankApp.Interfaces
{
    public interface IAccountService
    {
        IBankAccount CreateBankAccount(string name, AccountType accountType, string currency, decimal initalBalance);
        List<IBankAccount> GetAccounts();
        void GetAccounts(string modelName, string modelCurrency, decimal modelInitialBalance);
    }
}
