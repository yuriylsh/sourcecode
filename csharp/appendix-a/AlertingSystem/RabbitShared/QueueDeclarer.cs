using RabbitMQ.Client;

namespace RabbitShared
{
    public static class QueueDeclarer
    {
        public static string DeclareQueueCritical(IModel channel)
        {
            channel.QueueDeclare("critical", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind("critical", AlertsExchangeFactory.ExchangeName, "critical.*");
            return "critical";
        }

        public static string DeclareQueueRateLimit(IModel channel)
        {
            channel.QueueDeclare("rate_limit", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind("rate_limit", AlertsExchangeFactory.ExchangeName, "*.rate_limit");
            return "rate_limit";
        }
    }
}