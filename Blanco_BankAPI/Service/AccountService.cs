using System;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.Models;

namespace Blanco_BankAPI.Service
{
    public class AccountService : IAccountService
    {
        private readonly BlancoDbContext _context;

        public AccountService(BlancoDbContext context)
        {
            _context = context;
        }

        public int GetAccountAmountByUserId(int userId)
        {
            Account? account = _context.Accounts.FirstOrDefault(x => x.UserId == userId);
            return account.Balance;
        }

        public async Task<Account> CreateAccountBalance(int userId, int initialAmount, string accountNumber)
        {
            var newAccount = new Account
            {
                UserId = userId,
                Balance = initialAmount,
                AccountNumber = accountNumber,
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return newAccount;
        }
    }
}

