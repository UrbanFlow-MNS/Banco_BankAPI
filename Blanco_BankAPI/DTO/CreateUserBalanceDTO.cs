using System;
using MassTransit;

namespace Blanco_BankAPI.DTO
{
    public class CreateUserBalanceDTO
	{
		public int Id { get; set; }
		public int Balance { get; set; }
		public int UserId { get; set; }
		public string AccNumber { get; set; }
	}
}

