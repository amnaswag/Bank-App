namespace BankApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly List<IBankAccount> _accounts = new ();

        public IBankAccount CreateBankAccount(string name, AccountType accountType, string currency,
            decimal initalBalance)
        {
            var account = new BankAccount(name, accountType, currency, initalBalance);
            _accounts.Add(account);
            return account;
        }

    public List<IBankAccount> GetAccounts() => _accounts;
    public void GetAccounts(string modelName, string modelCurrency, decimal modelInitialBalance)
    {
        throw new NotImplementedException();
    }

    public void CreateAccount(string modelName, AccountType modelType, string modelCurrency, decimal modelInitialBalance)
    {
        throw new NotImplementedException();
    }
    }
}
