using System;
using System.Text;
using System.Text.Json;
using Blanco_BankAPI.DTO;
using Blanco_BankAPI.Service;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Transports;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

//http://localhost:3000/balance/1

namespace Blanco_BankAPI
{
    public class BalanceConsumer : IConsumer<WrappedMessage<UserBalanceDTO>> 
    {
        private readonly IAccountService _accountService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBus _bus;

        public BalanceConsumer(IAccountService account, IBus bus, IPublishEndpoint publishEndpoint)
        {
            _accountService = account;
            _bus = bus;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<WrappedMessage<UserBalanceDTO>> context)
        {
            try
            {
                var envelope = context.Message;

                //Console.WriteLine("Received NestJS Message:");
                //Console.WriteLine($"Pattern: {envelope.Pattern}");
                //Console.WriteLine($"Data: {envelope.Data}");
                //Console.WriteLine($"Id: {envelope.Id}");
                //Console.WriteLine($"CorrelationId: {context.CorrelationId}");

                if (context.TryGetPayload(out RabbitMqReceiveContext rabbitContext))
                {
                    var props = rabbitContext.Properties;
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

                    await SendDirectReply(replyTo, correlationId, response);


                    await _publishEndpoint.Publish(response, publishContext =>
                    {
                        publishContext.SetRoutingKey(replyTo);
                        publishContext.CorrelationId = Guid.Parse(correlationId);
                    });



                }



                Console.WriteLine("creation dto");


                //var endpoint = await context.GetSendEndpoint(new Uri($"queue:{replyTo}"));
                //await endpoint.Send(response);

                //Console.WriteLine("avant réponse");
                //Console.WriteLine(context.Message);


                //await context.RespondAsync<UserBalanceDTO>(response);

                //await context.Publish<UserBalanceDTO>(new BalanceResponseDTO
                //{
                //    Balance = balance,
                //    UserId = userId
                //});

                //await context.Publish(response);
                //Console.WriteLine($"Réponse publiée : {response.Balance}");

                Console.WriteLine("c envoyé");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task SendDirectReply(string replyTo, string correlationId, object response)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ContentType = "application/json";
            props.DeliveryMode = 1; // Non-persistent

            var json = JsonSerializer.Serialize(response);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: replyTo,
                basicProperties: props,
                body: body
            );

            Console.WriteLine($"Reply sent to {replyTo} with correlation {correlationId}");
        }

        }
    }

