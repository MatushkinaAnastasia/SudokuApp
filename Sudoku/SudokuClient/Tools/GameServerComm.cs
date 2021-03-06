using System.Diagnostics;
using System.Net;
using UtilsLibrary.Servers;
using System.Threading.Tasks;

namespace SudokuClient.Tools
{
	public static class GameServerComm
    {
        public static async Task<SocketClient> RunGameServerAsync(string nameOfRoom)
		{
            var ip = UtilsLibrary.NetworkUtils.GetMyIp();
            var port = UtilsLibrary.NetworkUtils.GetFreePort();
            //var port = 11000;
            var path = UtilsLibrary.PathWorker.GetPath("pathToGameServer");

            Process.Start(path, $"{ip} {port}");
            var client = new SocketClient(ip, port);
            await UtilsLibrary.Grpc.ClientGrpc.SendRoom(nameOfRoom, ip.ToString(), port.ToString());

            return client;
        }

		//отправить сообщение о запуске комнаты

		//отправить сообщение об изменении ячейки судоку

		//отправить сообщение об отсоединении клиента

		//класс отвечает за отправку сообщений при помощи сокетов
	}
}
