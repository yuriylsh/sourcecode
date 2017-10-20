using System.Text;
using RabbitMQ.Client;

namespace HelloWorld.Shared
{
    public static class MessageBuilder
    {
        public static (IBasicProperties MessageProperties, byte[] MessageBody) GetMessage(IModel channel, string messageString)
        {
            IBasicProperties messageProperties = channel.CreateBasicProperties();
            messageProperties.ContentType = "text/plain";
            return (messageProperties, Encoding.ASCII.GetBytes(messageString));
        }
    }
}