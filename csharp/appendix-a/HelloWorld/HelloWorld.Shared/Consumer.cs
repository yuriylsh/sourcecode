using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HelloWorld.Shared
{
    public static class Consumer
    {
        public static void ConsumeMessages(IModel channel, Action onShouldExit)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(Names.QueueName, autoAck: false, consumer: consumer);

            void OnMessageReceived(object sender, BasicDeliverEventArgs deliveryArgs)
            {
                var receivingChannel = ((EventingBasicConsumer)sender).Model;
                receivingChannel.BasicAck(deliveryArgs.DeliveryTag, multiple: false);

                var message = Encoding.ASCII.GetString(deliveryArgs.Body);
                if(string.Equals("quit", message, StringComparison.OrdinalIgnoreCase))
                {
                    receivingChannel.BasicCancel(deliveryArgs.ConsumerTag);
                    onShouldExit();
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }
    }
}