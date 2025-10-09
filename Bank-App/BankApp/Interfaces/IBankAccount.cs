using Bank_App;

namespace Bank-App.Interfaces;
/// <summary>
/// Interface containing the BankAccount methods
/// </summary>
public class IBankAccount


public interface IBankAccount 
{
    Guid Id { get;  }
    string Name { get;  }
    decimal Balance { get;  } 
    DateTime LastUpdated { get;  }

    void Withdrawn(decimal amount);
    void Deposit(decimal amount);

}