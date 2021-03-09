namespace UtilsLibrary.Servers
{
	public interface IMessageHandler
	{
		void Handle(byte[] message, System.Net.Sockets.Socket socket);
	}
}
