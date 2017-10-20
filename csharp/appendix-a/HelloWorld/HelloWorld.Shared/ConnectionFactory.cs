using RabbitMQ.Client;

namespace HelloWorld.Shared
{
    public static class ConnectionFactory
    {
        public static IConnection CreateConnection() => new RabbitMQ.Client.ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }.CreateConnection();
    }
}