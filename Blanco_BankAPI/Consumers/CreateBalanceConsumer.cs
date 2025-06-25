using System;
using System.Security.Principal;
using System.Text.Json;
using Blanco_BankAPI.DTO;
using Blanco_BankAPI.Helpers;
using Blanco_BankAPI.Models;
using Blanco_BankAPI.Service;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Transports;

namespace Blanco_BankAPI.Consumers
{
	public class CreateBalanceConsumer : IConsumer<WrappedMessage<CreateUserBalanceDTO>>
    {
        private readonly IAccountService _accountService;

        public CreateBalanceConsumer(IAccountService account)
		{
            _accountService = account;
        }

        public async Task Consume(ConsumeContext<WrappedMessage<CreateUserBalanceDTO>> context)
        {
            try
            {

                if (context.TryGetPayload(out RabbitMqReceiveContext rabbitContext))
                {
                    Console.WriteLine("Payload reçu : " + JsonSerializer.Serialize(context.Message.Data));

                    var props = rabbitContext.Properties;

                    Console.WriteLine("props: " + props);
                    Console.WriteLine($"ReplyTo: {props.ReplyTo}");
                    Console.WriteLine($"CorrelationId: {props.CorrelationId}");


                    string replyTo = props.ReplyTo;
                    string correlationId = props.CorrelationId;

                    int userId = context.Message.Data.UserId;
                    int amount = context.Message.Data.Balance;
                    string accNumber = context.Message.Data.AccNumber;

                    _accountService.CreateAccountBalance(userId, amount, accNumber);

                    Console.WriteLine("Balance created with success");
                    await SendDirectReplyHelper.SendDirectReply(replyTo, correlationId, new
                    {
                        success = true,

                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

