using RabbitMQ.Client;
using System.Text;

namespace UtilsLibrary.RabbitMQ
{
	public class RabbitMQClient
	{
		private ConnectionFactory _factory;
		private const string ChatExchange = "chex";
		public RabbitMQClient(string hostName)
		{
			_factory = new ConnectionFactory() { HostName = hostName, UserName = "trrp4", Password = "trrp4" };
		}

		public void Send(string message)
		{
			using var connection = _factory.CreateConnection();
			using var channel = connection.CreateModel();

			//channel.QueueDeclare("hello", false, false, false, null);
			channel.ExchangeDeclare(exchange: ChatExchange, type: "fanout");
			var body = Encoding.UTF8.GetBytes(message);
			channel.BasicPublish(ChatExchange, "", null, body);
		}
	}
}
