using RabbitMQ.Client;
using System.Configuration;
using System.Text;

namespace UtilsLibrary.RabbitMQ
{
	public class RabbitMQClient
	{
		private readonly ConnectionFactory _factory;
		private const string ChatExchange = "chex";
		public RabbitMQClient(string hostName)
		{
			var username = ConfigurationManager.AppSettings.Get("usernameMQ");
			var password = ConfigurationManager.AppSettings.Get("passwordMQ");
			_factory = new ConnectionFactory() { HostName = hostName, UserName = username, Password = password };
		}

		public void Send(string message, string nickname)
		{
			using var connection = _factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.ExchangeDeclare(exchange: ChatExchange, type: "fanout");
			var body = Encoding.UTF8.GetBytes(message + "`" + nickname);
			channel.BasicPublish(ChatExchange, "", null, body);
		}
	}
}
