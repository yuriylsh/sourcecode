using RabbitMQ.Client;

namespace RabbitShared
{
    public static class AlertsExchangeFactory
    {
        public const string ExchangeName = "alerts";

        public static void DeclareAlertsExhange(IModel channel) => channel.ExchangeDeclare(
            ExchangeName,
            ExchangeType.Topic,
            durable: false,
            autoDelete:false,
            arguments: null);
    }
}