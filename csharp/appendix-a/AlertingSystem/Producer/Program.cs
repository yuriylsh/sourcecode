using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using RabbitShared;

namespace Producer
{
    internal class Program
    {
        static void Main()
        {
            var connection = RabbitConnectionSingleton.Get();
            var channel = connection.CreateModel();
            EnsureExchangeAndQueues(channel);
            StartSendingMessages(channel);
        }

        private static void EnsureExchangeAndQueues(IModel channel)
        {
            AlertsExchangeFactory.DeclareAlertsExhange(channel);
            QueueDeclarer.DeclareQueueRateLimit(channel);
            QueueDeclarer.DeclareQueueCritical(channel);
        }

        private static void StartSendingMessages(IModel channel)
        {
            Console.WriteLine("Enter message ('quit' to exti):");
            string userInput = Console.ReadLine();
            bool shouldQuit = string.Equals("quit", userInput, StringComparison.OrdinalIgnoreCase);
            if (!shouldQuit)
            {
                SendMessage(userInput, channel);
                StartSendingMessages(channel);
            }
        }

        private static void SendMessage(string message, IModel channel)
        {
            var routingKey = RoutingKeyGenerator.Generate();
            IBasicProperties messageProperties = new BasicProperties
            {
                ContentType = "text/plain",
                Persistent = false
            };
            Console.WriteLine("PUBLISHING MESSAGE: routing key = {0}, message body = {1}", routingKey, message);
            channel.BasicPublish(AlertsExchangeFactory.ExchangeName, routingKey, basicProperties: messageProperties, body: Encoding.ASCII.GetBytes(message));
        }
    }
}
