using System;
using System.Text;
using HelloWorld.Shared;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ConnectionFactory = HelloWorld.Shared.ConnectionFactory;

namespace PublishWithConfirmation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connection = ConnectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            ExchangeDeclarer.DeclareExchange(channel);
            QueueDeclarer.DeclareQueue(channel);
            channel.QueueBind(Names.QueueName, Names.ExchangeName, Names.RoutingKey, null);

            var publishMonitor = new PublishMonitor();
            channel.ConfirmSelect();
            channel.BasicAcks += publishMonitor.OnBasicAck;
            channel.BasicNacks += publishMonitor.OnBasicNack;

            var commandLineArgs = CommandLineArgs.Parse(args);
            var (messageProperties, messageBody) = MessageBuilder.GetMessage(channel, commandLineArgs.Message);
            for (int i = 0; i < commandLineArgs.Count; i++)
            {
                publishMonitor.NewMessagePublished();
                channel.BasicPublish(
                    Names.ExchangeName,
                    Names.RoutingKey,
                    basicProperties: messageProperties,
                    body: messageBody); 
            }

            ConsumeMessages(channel);
        }

        public static void ConsumeMessages(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            
            channel.BasicConsume(Names.QueueName, autoAck: false, consumer: consumer);

            void OnMessageReceived(object sender, BasicDeliverEventArgs deliveryArgs)
            {
                var messageConsumer = (EventingBasicConsumer) sender;
                var receivingChannel = messageConsumer.Model;
                receivingChannel.BasicAck(deliveryArgs.DeliveryTag, multiple: false);

                var message = Encoding.ASCII.GetString(deliveryArgs.Body);
                Console.WriteLine("Message {0}: {1}", deliveryArgs.DeliveryTag, message);
            }
        }
    }
}
