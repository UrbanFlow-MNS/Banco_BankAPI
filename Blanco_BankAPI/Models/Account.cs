using System;
namespace Blanco_BankAPI.Models
{
	public class Account
	{
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public int Balance { get; set; }
        public bool IsOverdraftable { get; set; }
        public int UserId { get; set; }
    }
}

