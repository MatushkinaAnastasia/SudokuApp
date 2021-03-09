using RabbitMQ.Client;
using System.Text;

namespace UtilsLibrary.RabbitMQ
{
	public class RabbitMQClient
	{
		private ConnectionFactory _factory;
		public RabbitMQClient(string hostName)
		{
			_factory = new ConnectionFactory() { HostName = hostName };
		}

		public void Send(string message)
		{
			using var connection = _factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.QueueDeclare("hello", false, false, false, null);
			var body = Encoding.UTF8.GetBytes(message);
			channel.BasicPublish("", "hello", null, body);
		}
	}
}
