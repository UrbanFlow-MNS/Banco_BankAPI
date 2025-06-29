using System;
using MassTransit;

namespace Blanco_BankAPI.DTO
{
    public class BalanceDTO
	{
		public int UserId { get; set; }
		public int Balance { get; set; }
		public string AccNumber { get; set; }

	}
}


