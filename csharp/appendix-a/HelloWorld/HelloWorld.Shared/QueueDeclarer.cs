using RabbitMQ.Client;

namespace HelloWorld.Shared
{
    public static class QueueDeclarer
    {
        public static void DeclareQueue(IModel channel) => channel.QueueDeclare(
            Names.QueueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);
    }
}