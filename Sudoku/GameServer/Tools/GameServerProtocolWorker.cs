using System;
using System.Linq;
using System.Net;
using System.Text;
using UtilsLibrary.Data;

namespace GameServer.Tools
{
	public static class GameServerProtocolWorker
	{
		public static Client GetClient(byte[] message, int startIndex)
		{
			var ipB = new byte[4];
			Array.Copy(message, startIndex, ipB, 0, 4);
			var portB = new byte[4];
			Array.Copy(message, startIndex + 4, portB, 0, 4);

			var ip = new IPAddress(ipB);
			var port = BitConverter.ToInt32(portB, 0);

			return new Client(ip, port);
		}

		public static byte[] GetDataAndNameBytes(SudokuCell[,] data, string nameOfRoom)
		{
			var dataBytes = data.ConvertToByteArray();
			var nameBytes = Encoding.UTF8.GetBytes(nameOfRoom);
			var nameLengthBytes = BitConverter.GetBytes(nameBytes.Length);

			return dataBytes.Concat(nameLengthBytes.Concat(nameBytes)).ToArray();
		}

		public static (byte x, byte y, byte value) GetXYValue(byte[] message, int startIndex)
		{
			var x = message[startIndex];
			var y = message[startIndex + 1];
			var value = message[startIndex + 2];
			return (x, y, value);
		}
	}
}
