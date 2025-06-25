using System;
using MassTransit;

namespace Blanco_BankAPI.DTO
{
    [MessageUrn("UserBalanceDTO")]
    public class GetUserBalanceDTO
	{
		public int UserId { get; set; }
	}
}


