using System.Diagnostics;
using UtilsLibrary.Servers;
using UtilsLibrary;

namespace SudokuClient.Tools
{
	public static class GameServerComm
    {
        public static SocketClient RunGameServer(string nameOfRoom, string pathToFile)
		{
            var ip = NetworkUtils.GetMyIp();
            var port = NetworkUtils.GetFreePort();

            var path = PathWorker.GetPath("pathToGameServer");
            Process.Start(path, $"{ip} {port} {nameOfRoom.Replace(" ", "_")} {pathToFile.Replace(" ","?")}");

            var client = new SocketClient(ip, port);
            return client;
        }
	}
}
