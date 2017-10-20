using HelloWorld.Shared;
using RabbitMQ.Client;
using static HelloWorld.Shared.Names;
using static HelloWorld.Shared.ConnectionFactory;
using static HelloWorld.Shared.ExchangeDeclarer;
using static HelloWorld.Shared.QueueDeclarer;
using static HelloWorld.Shared.MessageBuilder;

namespace ProducerAndConsumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connection = CreateConnection();
            var channel = connection.CreateModel();
            DeclareExchange(channel);
            DeclareQueue(channel);
            channel.QueueBind(QueueName, ExchangeName, RoutingKey, arguments: null);

            var commandLineArgs = CommandLineArgs.Parse(args);
            var (messageProperties, messageBody) = GetMessage(channel, commandLineArgs.Message);
            for (var i = 0; i < commandLineArgs.Count; i++)
            {
                channel.BasicPublish(
                    ExchangeName,
                    RoutingKey,
                    basicProperties: messageProperties,
                    body: messageBody); 
            }

            Consumer.ConsumeMessages(channel);
        }
    }
}
