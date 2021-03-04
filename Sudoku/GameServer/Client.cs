using System.Net;
using UtilsLibrary.Servers;

namespace GameServer
{
	internal class Client
	{
		public Client(IPAddress ip, int port)
		{
			IP = ip;
			Port = port;
			SocketClient = new SocketClient(ip, port);
		}

		public IPAddress IP { get; private set; }
		public int Port { get; private set; }

		public SocketClient SocketClient { get; private set; }
	}
}