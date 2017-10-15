using System;
using RabbitMQ.Client;
using RabbitShared;

namespace Consumer
{
    internal class Program
    {
        static void Main()
        {
            var connection = RabbitConnectionSingleton.Get();
            var channel = connection.CreateModel();
            AlertsExchangeFactory.DeclareAlertsExhange(channel);
            var criticalQueueName = QueueDeclarer.DeclareQueueCritical(channel);
            var rateLimitQueueName = QueueDeclarer.DeclareQueueRateLimit(channel);

            channel.BasicConsume(
                criticalQueueName,
                autoAck: false,
                consumer: new CriticalQueueConsumer(channel),
                consumerTag: "critical"
            );
            channel.BasicConsume(
                rateLimitQueueName,
                autoAck: false,
                consumer: new RateLimitQueueConsumer(channel),
                consumerTag: "rate_limit");

            Console.WriteLine("Ready for alerts!");
        }
    }
}