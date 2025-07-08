using System;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Blanco_BankAPI.Helpers
{
	public class SendDirectReplyHelper
	{
        /// <summary>
        /// Helper to send a response to API gateway sender because i can't get reply_to property with MassTransit somehow 
        /// </summary>
        /// <param name="replyTo"></param>
        /// <param name="correlationId"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task SendDirectReply(string replyTo, string correlationId, object response)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "password" };
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

