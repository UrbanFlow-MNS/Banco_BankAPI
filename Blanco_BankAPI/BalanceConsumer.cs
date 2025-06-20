using System;
using Blanco_BankAPI.DTO;
using Blanco_BankAPI.Service;
using MassTransit;

namespace Blanco_BankAPI
{
    public class BalanceConsumer : IConsumer<WrappedMessage<UserBalanceDTO>>, IBalanceConsumer
    {
        private readonly IAccountService _accountService;

        public BalanceConsumer(IAccountService account)
        {
            _accountService = account;
        }

        public async Task Consume(ConsumeContext<WrappedMessage<UserBalanceDTO>> context)
        {
            var userId = context.Message.Data.UserId;
            Console.WriteLine(userId);
            var balance = _accountService.GetAccountAmountByUserId(userId);

            Console.WriteLine($"Processing user balance request for UserId: {userId}");
            Console.WriteLine($"Message: {System.Text.Json.JsonSerializer.Serialize(context.Message)}");

            await context.RespondAsync(balance);
        }
    }
}

