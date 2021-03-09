namespace UtilsLibrary.RabbitMQ
{
	public interface IMessageHandler
	{
		void Handle(string message);
	}
}
