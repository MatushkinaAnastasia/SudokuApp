using System.Diagnostics;
using UtilsLibrary.Servers;
using UtilsLibrary;

namespace SudokuClient.Tools
{
	public static class GameServerComm
    {
        public static SocketClient RunGameServer(string nameOfRoom)
		{
            var ip = NetworkUtils.GetMyIp();
            var port = NetworkUtils.GetFreePort();

            var path = PathWorker.GetPath("pathToGameServer");
            Process.Start(path, $"{ip} {port} {nameOfRoom.Replace(" ", "_")}");

            var client = new SocketClient(ip, port);
            return client;
        }
	}
}
