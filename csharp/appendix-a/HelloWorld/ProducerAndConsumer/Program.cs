using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

using static HelloWorld.Shared.Names;
using static HelloWorld.Shared.ConnectionFactory;
using static HelloWorld.Shared.ExchangeDeclarer;
using static HelloWorld.Shared.QueueDeclarer;
using static HelloWorld.Shared.Consumer;
using static HelloWorld.Shared.MessageBuilder;

namespace ProducerAndConsumer
{
    internal class Program
    {
        private static bool _shouldExitApp = false;

        static async Task Main(string[] args)
        {
            var connection = CreateConnection();
            IModel channel = connection.CreateModel();
            DeclareExchange(channel);
            DeclareQueue(channel);
            channel.QueueBind(QueueName, ExchangeName, RoutingKey, null);
            
            var (messageProperties, messageBody) = GetMessage(channel, GetMessageFromCommandLineArguments(args));
            channel.BasicPublish(
                ExchangeName,
                RoutingKey,
                basicProperties: messageProperties,
                body: messageBody);

            ConsumeMessages(channel, () => _shouldExitApp = true);
            await WaitForExit();
        }

        private static string GetMessageFromCommandLineArguments(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Must supply message text.");
                Environment.Exit(-1);
            }
            return args[0];
        }

        private static async Task WaitForExit()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            while (!_shouldExitApp) await Task.Delay(delay);
            Environment.Exit(0);
        }
    }
}
