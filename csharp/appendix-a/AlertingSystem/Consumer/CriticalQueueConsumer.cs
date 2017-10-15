using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    internal class CriticalQueueConsumer : EventingBasicConsumer
    {
        public CriticalQueueConsumer(IModel channel) : base(channel)
        {
        }

        public override void HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            byte[] body)
        {
            Console.WriteLine("CRITICAL NOTIFICATION: {0}", Encoding.ASCII.GetString(body));
            Model.BasicAck(deliveryTag, multiple: false);
        }
    }
}