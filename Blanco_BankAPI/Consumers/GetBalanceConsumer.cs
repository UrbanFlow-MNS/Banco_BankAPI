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
    public class GetBalanceConsumer : IConsumer<WrappedMessage<GetUserBalanceDTO>> 
    {
        private readonly IAccountService _accountService;
        private readonly IPublishEndpoint _publishEndpoint;

        public GetBalanceConsumer(IAccountService account, IPublishEndpoint publishEndpoint)
        {
            _accountService = account;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<WrappedMessage<GetUserBalanceDTO>> context)
        {
            Console.WriteLine(context.Message.Pattern);

            try
            {
                if (context.TryGetPayload(out RabbitMqReceiveContext rabbitContext))
                {
                    var props = rabbitContext.Properties;

                    Console.WriteLine("APPEL DU CONSUMER DE VA CHERCHER BALANCE POUR UTILISATEUR");

                    Console.WriteLine("props: " + props);
                    Console.WriteLine($"ReplyTo: {props.ReplyTo}");
                    Console.WriteLine($"CorrelationId: {props.CorrelationId}");


                    string replyTo = props.ReplyTo;
                    string correlationId = props.CorrelationId;

                    int userId = context.Message.Data.UserId;

                    int balance = _accountService.GetAccountAmountByUserId(context.Message.Data.UserId);

                    Console.WriteLine("userid: " + userId + " balance: " + balance);


                    var response = new BalanceResponseDTO
                    {
                        Balance = balance,
                        UserId = userId
                    };

                    await SendDirectReplyHelper.SendDirectReply(replyTo, correlationId, response);
                    Console.WriteLine("c envoyé");
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

