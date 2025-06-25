using Blanco_BankAPI.Models;

namespace Blanco_BankAPI.Service
{
    public interface IAccountService
    {
        int GetAccountAmountByUserId(int userId);
        Task<Account> CreateAccountBalance(int userId, int initialAmount, string accountNumber);

    }
}