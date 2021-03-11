using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UtilsLibrary.Data;

namespace SudokuClient.Tools
{
	public static class GameServerProtocolWorker
	{
		public static byte[] GetAddressBytes(IPAddress ip, int port)
		{
			var ipBytes = ip.GetAddressBytes();
			var portBytes = BitConverter.GetBytes(port);
			return new byte[]
			{
				ipBytes[0], ipBytes[1], ipBytes[2], ipBytes[3],
				portBytes[0], portBytes[1], portBytes[2], portBytes[3],
			};
		}

		public static (SudokuCell[,] data, string name) ConvertToSudokuCellArrayAndRoomName(byte[] bytes)
		{
			var dataSudoku = bytes.ConvertToSudokuCellArray();
			var sizeOfOneSudokuCell = new SudokuCell(1, 1, 1, true).ToByteArray().Length;
			var dataBytesLength = 9 * 9 * sizeOfOneSudokuCell;
			var nameBytesLength = BitConverter.ToInt32(bytes, dataBytesLength);

			var name = Encoding.UTF8.GetString(bytes, dataBytesLength + 4, nameBytesLength);

			return (dataSudoku, name);
		}
	}
}
