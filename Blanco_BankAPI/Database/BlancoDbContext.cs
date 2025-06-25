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

    }
}

