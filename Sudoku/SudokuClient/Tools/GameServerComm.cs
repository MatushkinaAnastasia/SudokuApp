using System.Diagnostics;
using System.Net;
using UtilsLibrary.Servers;

namespace SudokuClient.Tools
{
	public static class GameServerComm
    {
        public static SocketClient RunGameServer()
		{
            var hostName = Dns.GetHostName();
            var ip = Dns.GetHostEntry(hostName).AddressList[1];
            var port = UtilsLibrary.NetworkUtils.GetFreePort();
            //var port = 11000;
            var path = UtilsLibrary.PathWorker.GetPath("pathToGameServer");

            Process.Start(path, $"{ip} {port}");
            var client = new SocketClient(ip, port);

            return client;
        }

		//отправить сообщение о запуске комнаты

		//отправить сообщение об изменении ячейки судоку

		//отправить сообщение об отсоединении клиента

		//класс отвечает за отправку сообщений при помощи сокетов
	}
}
