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
            Account? account = _context.accounts.FirstOrDefault(x => x.UserId == userId);
            return account.Balance;
        }
    }
}

