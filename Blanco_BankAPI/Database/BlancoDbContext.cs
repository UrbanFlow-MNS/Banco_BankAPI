using System;
using System.Numerics;
using Blanco_BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Blanco_BankAPI.Database
{
	public class BlancoDbContext : DbContext
	{
        public BlancoDbContext(DbContextOptions<BlancoDbContext> options)
        : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public List<Account> accounts = new List<Account>
        {
            new Account { AccountNumber = "FR001", Balance = 1500, UserId = 1 },
            new Account { AccountNumber = "FR002", Balance = 3000, UserId = 2 },
            new Account { AccountNumber = "FR003", Balance = 500, UserId = 3 }
        };
    }
}

