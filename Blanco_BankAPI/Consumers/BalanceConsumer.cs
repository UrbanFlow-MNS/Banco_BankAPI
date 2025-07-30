using System;
using System.Text;
using System.Text.Json;
using Blanco_BankAPI.DTO;
using Blanco_BankAPI.Helpers;
using Blanco_BankAPI.Service;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Transports;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

//http://localhost:3000/balance/1

namespace Blanco_BankAPI
{
    public class BalanceConsumer : IConsumer<WrappedMessage<BalanceDTO>> 
    {
        private readonly IAccountService _accountService;

        public BalanceConsumer(IAccountService account)
        {
            _accountService = account;
        }

        public async Task Consume(ConsumeContext<WrappedMessage<BalanceDTO>> context)
        {

            Console.WriteLine("patern: " + context.Message.Pattern);
            context.TryGetPayload(out RabbitMqReceiveContext rabbitContext);
            var props = rabbitContext.Properties;
            string replyTo = props.ReplyTo;
            string correlationId = props.CorrelationId;

            string pattern = context.Message.Pattern;

            try
            {
                switch (pattern)
                {
                    case "GetUserBalance":
                        Console.WriteLine("GET USER BALANCE :");
                        int userId = context.Message.Data.UserId;
                        int balance = _accountService.GetAccountAmountByUserId(context.Message.Data.UserId);
                        var response = new BalanceResponseDTO
                        {
                            Balance = balance,
                            UserId = userId
                        };


                        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        });

                        await SendDirectReplyHelper.SendDirectReply(replyTo, correlationId, json);
                        break;
                    case "CreateUserBalance":
                        Console.WriteLine("CREATE USER BALANCE :");
                        int userIdToCreate = context.Message.Data.UserId;
                        int amountToCreate = context.Message.Data.Balance;
                        string accNumberToCreate = context.Message.Data.AccNumber;

                        await _accountService.CreateAccountBalance(userIdToCreate, amountToCreate, accNumberToCreate);
                        await SendDirectReplyHelper.SendDirectReply(replyTo, correlationId, new
                        {
                            success = true,

                        });
                        break;
                    default:
                        Console.WriteLine("pas marché");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

