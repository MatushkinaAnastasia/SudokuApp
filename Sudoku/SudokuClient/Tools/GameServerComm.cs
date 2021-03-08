using System.Diagnostics;
using System.Net;
using UtilsLibrary.Servers;
using System.Threading.Tasks;
using System;

namespace SudokuClient.Tools
{
	public static class GameServerComm
    {
        public static async Task<SocketClient> RunGameServerAsync(string nameOfRoom)
		{
            var ip = UtilsLibrary.NetworkUtils.GetMyIp();
            var port = UtilsLibrary.NetworkUtils.GetFreePort();
            var path = UtilsLibrary.PathWorker.GetPath("pathToGameServer");

            try
			{
                Process.Start(path, $"{ip} {port} {nameOfRoom.Replace(" ", "_")}");
            }
            catch
			{
                throw new Exception("Проблемы создания комнаты.");
			}

            var client = new SocketClient(ip, port);
            
			try
			{
                await UtilsLibrary.Grpc.ClientGrpc.SendRoom(nameOfRoom, ip.ToString(), port.ToString());
            }
            catch
			{
                throw new Exception("сервером.");
			}

            return client;
        }
	}
}
