using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using RabbitShared;

namespace Producer
{
    internal class Program
    {
        private const string QuitMarker = "quit";

        private static readonly IBasicProperties MessageProperties = new BasicProperties
        {
            ContentType = "text/plain",
            Persistent = false
        };

        static void Main()
        {
            var connection = RabbitConnectionSingleton.Get();
            var channel = connection.CreateModel();
            EnsureExchangeAndQueues(channel);
            ProcessMessagesFromUser(channel);
        }

        private static void EnsureExchangeAndQueues(IModel channel)
        {
            AlertsExchangeFactory.DeclareAlertsExhange(channel);
            QueueDeclarer.DeclareQueueRateLimit(channel);
            QueueDeclarer.DeclareQueueCritical(channel);
        }

        private static void ProcessMessagesFromUser(IModel channel)
        {
            var (message, quit) = GetMessageFromUser();
            if (quit) return;

            SendMessageToChannel(message, channel);
            ProcessMessagesFromUser(channel);
        }

        private static (string Message, bool quit) GetMessageFromUser()
        {
            Console.WriteLine("Enter message ('{0}' to exit):", QuitMarker);
            string userInput = Console.ReadLine();
            return (userInput, string.Equals(QuitMarker, userInput, StringComparison.OrdinalIgnoreCase));
        }

        private static void SendMessageToChannel(string message, IModel channel)
        {
            var routingKey = RoutingKeyGenerator.Generate();
            
            Console.WriteLine("PUBLISHING MESSAGE: routing key = {0}, message body = {1}", routingKey, message);
            channel.BasicPublish(AlertsExchangeFactory.ExchangeName, routingKey, basicProperties: MessageProperties, body: Encoding.ASCII.GetBytes(message));
        }
    }
}
