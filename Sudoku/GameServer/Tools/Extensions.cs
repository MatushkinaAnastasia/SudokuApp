using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GameServer.Tools
{
	public static class Extensions
	{
		public static bool IsEquals(this Client client1, Client client2)
		{
			var result = client1.IP.ToString() == client2.IP.ToString() && client1.Port == client2.Port;
			return result;
		}
	}
}
