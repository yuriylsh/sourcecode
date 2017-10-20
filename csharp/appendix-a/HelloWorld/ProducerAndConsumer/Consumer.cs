using System;
using System.Text;
using HelloWorld.Shared;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProducerAndConsumer
{
    internal static class Consumer
    {
        public static void ConsumeMessages(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(Names.QueueName, autoAck: false, consumer: consumer);

            void OnMessageReceived(object sender, BasicDeliverEventArgs deliveryArgs)
            {
                var receivingChannel = ((EventingBasicConsumer)sender).Model;
                receivingChannel.BasicAck(deliveryArgs.DeliveryTag, multiple: false);
                Console.WriteLine("Message {0}: {1}", deliveryArgs.DeliveryTag, Encoding.ASCII.GetString(deliveryArgs.Body));
            }
        }
    }
}