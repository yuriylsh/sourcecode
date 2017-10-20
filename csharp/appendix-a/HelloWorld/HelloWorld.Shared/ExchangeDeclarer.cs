using RabbitMQ.Client;

namespace HelloWorld.Shared
{
    public static class ExchangeDeclarer
    {
        public static void DeclareExchange(IModel channel) => channel.ExchangeDeclare(
            Names.ExchangeName,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null);
    }
}