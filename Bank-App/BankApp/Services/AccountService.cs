namespace BankApp.Services
{
    public class AccountService : IAccountService
    {
        public IBankAccount CreateBankAccount(string name, string currency, decimal initalBalance)
        {
            throw new NotImplementedException();
        }

        public List<IBankAccount> GetAccounts()
        {
            throw new NotImplementedException();
        }
    }
}
