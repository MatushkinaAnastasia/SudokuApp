using System.Net;
using System.Net.Sockets;

namespace UtilsLibrary
{
	public static class NetworkUtils
	{
		public static int GetFreePort()
		{
			TcpListener l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();
			int port = ((IPEndPoint)l.LocalEndpoint).Port;
			l.Stop();
			return port;
		}
	}
}
