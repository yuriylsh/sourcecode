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
            string criticalQueueName = QueueDeclarer.DeclareQueueCritical(channel);
            string rateLimitQueueName = QueueDeclarer.DeclareQueueRateLimit(channel);

            channel.BasicConsume(
                criticalQueueName, 
                autoAck: false, 
                consumerTag: "critical",
                consumer: new CriticalQueueConsumer());
            channel.BasicConsume(
                rateLimitQueueName, 
                autoAck: false, 
                consumerTag: "critical",
                consumer: new RateLimitQueueConsumer());
        }
    }
}
