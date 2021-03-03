namespace GameServer
{
	public interface IMessageHandler
	{
		void Handle(byte[] message);
	}
}