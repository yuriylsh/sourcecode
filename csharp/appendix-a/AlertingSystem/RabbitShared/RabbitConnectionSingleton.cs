using RabbitMQ.Client;

namespace RabbitShared
{
    public static class RabbitConnectionSingleton
    {
        private static IConnection _connection;

		public static IConnection Get() => _connection ?? (_connection = CreateConnection());

        private static IConnection CreateConnection() => new ConnectionFactory
        {
            HostName =  "localhost",
            UserName = "guest",
            Password = "guest"
        }.CreateConnection();
    }
}
