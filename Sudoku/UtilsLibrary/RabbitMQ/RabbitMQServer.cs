using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLibrary.Servers;

namespace UtilsLibrary.RabbitMQ
{
	public class RabbitMQServer
	{
		private readonly ConnectionFactory _factory;
        private const string ChatExchange = "chex";
        public RabbitMQServer(string hostName)
		{
			_factory = new ConnectionFactory() { HostName = hostName, UserName = "trrp4", Password = "trrp4"};
		}

        public async void Run(IMessageHandler handler, CancellationToken cancellationToken)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: ChatExchange, type: "fanout");

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName, ChatExchange, "");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                handler.Handle(message);
            };

            var consumerTag = channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            await Task.Run(() =>
            {
                cancellationToken.WaitHandle.WaitOne();
                //channel.BasicCancel(consumerTag);
				Console.WriteLine("consumer closed");
            });
        }
    }
}








//public void GetMessage()
//{
//	var factory = new ConnectionFactory() { HostName = "localhost" };
//	using var connection = factory.CreateConnection();
//	using var channel = connection.CreateModel();

//	channel.QueueDeclare(queue: "hello",
//						 durable: false,
//						 exclusive: false,
//						 autoDelete: false,
//						 arguments: null);

//	var consumer = new EventingBasicConsumer(channel);
//	consumer.Received += (model, ea) =>
//	{
//		var body = ea.Body.ToArray();
//		var message = Encoding.UTF8.GetString(body);
//		//вывести в чат message
//	};

//	channel.BasicConsume(queue: "hello",
//						 autoAck: true,
//						 consumer: consumer);


//}