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

		public static IPAddress GetMyIp()
		{
			var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

			foreach (var address in addressList)
			{
				if (address.AddressFamily == AddressFamily.InterNetwork)
				{
					return address;
				}
			}

			return null;
		}

		public static byte[] ReadNBytes(this NetworkStream stream, int size)
		{
			byte[] bytes = new byte[size];
			int readTotal = 0;
			while (true)
			{
				int left = size - readTotal;
				if (left <= 0)
				{
					break;
				}
				readTotal += stream.Read(bytes, readTotal, left);
			}
			return bytes;
		}
	}
}
