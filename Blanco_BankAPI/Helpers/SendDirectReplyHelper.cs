using System;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Blanco_BankAPI.Helpers
{
	public class SendDirectReplyHelper
	{
        public static async Task SendDirectReply(string replyTo, string correlationId, object response)
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

