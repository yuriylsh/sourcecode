using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProducerAndConsumer
{
    internal class Program
    {
        private const string ExchangeName = "hello-exchange";
        private const string RoutingKey = "hola";
        private const string QueueName = "hello-queue";
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


            ConsumeMessages(channel);
            await WaitForExit();
        }

        private static IConnection CreateConnection() => new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }.CreateConnection();

        private static void DeclareExchange(IModel channel) => channel.ExchangeDeclare(
            ExchangeName,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null);

        private static void DeclareQueue(IModel channel) => channel.QueueDeclare(
            QueueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);

        private static (IBasicProperties MessageProperties, byte[] MessageBody) GetMessage(IModel channel, string messageString)
        {
            IBasicProperties messageProperties = channel.CreateBasicProperties();
            messageProperties.ContentType = "text/plain";
            return (messageProperties, Encoding.ASCII.GetBytes(messageString));
        }

        private static string GetMessageFromCommandLineArguments(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Must supply hostname and message text.");
                Environment.Exit(-1);
            }
            return args[0];
        }

        private static void ConsumeMessages(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);

            void OnMessageReceived(object sender, BasicDeliverEventArgs deliveryArgs)
            {
                var receivingChannel = ((EventingBasicConsumer)sender).Model;
                receivingChannel.BasicAck(deliveryArgs.DeliveryTag, multiple: false);

                var message = Encoding.ASCII.GetString(deliveryArgs.Body);
                if(string.Equals("quit", message, StringComparison.OrdinalIgnoreCase))
                {
                    receivingChannel.BasicCancel(deliveryArgs.ConsumerTag);
                    _shouldExitApp = true;
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        private static async Task WaitForExit()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            while (!_shouldExitApp) await Task.Delay(delay);
            Environment.Exit(0);
        }
    }
}
