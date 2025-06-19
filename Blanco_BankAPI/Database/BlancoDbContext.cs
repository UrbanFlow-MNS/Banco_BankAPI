using System;
using System.Numerics;
using Blanco_BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Blanco_BankAPI.Database
{
	public class BlancoDbContext
	{
        public List<Account> accounts = new List<Account>
        {
            new Account { AccountNumber = "FR001", Balance = 1500.00m, UserId = 1 },
            new Account { AccountNumber = "FR002", Balance = 3000.00m, UserId = 2 },
            new Account { AccountNumber = "FR003", Balance = 500.00m, UserId = 3 }
        };
    }
}

