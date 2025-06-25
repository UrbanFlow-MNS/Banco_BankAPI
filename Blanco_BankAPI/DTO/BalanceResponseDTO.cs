using System;
using MassTransit;

namespace Blanco_BankAPI.DTO
{
	public class BalanceResponseDTO
	{
		public string Id { get; set; }
		public int Balance { get; set; }
		public int UserId { get; set; }
	}
}


