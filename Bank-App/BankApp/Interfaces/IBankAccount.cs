using BankApp;

namespace BankApp.Interfaces;
/// <summary>
/// Interface containing the BankAccount methods
/// </summary>



public interface IBankAccount 
{
    Guid Id { get;  }
    string Name { get;  }
    string Currency { get;  }
    decimal Balance { get;  } 
    DateTime LastUpdated { get;  }

    void Withdrawn(decimal amount);
    void Deposit(decimal amount);

}