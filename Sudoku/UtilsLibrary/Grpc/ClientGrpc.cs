using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Grpc.Net.Client;

namespace UtilsLibrary.Grpc
{
	public class ClientGrpc
	{
		public static async Task SendRoom(string name, string ip, string port)
		{
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); //нешифрованные данные

			//TODO: читать ip/порт из конфига
			var address = $"http://{NetworkUtils.GetMyIp()}:5000"; //работает если grpc сервер запускается на том же компе!
			using var channel = GrpcChannel.ForAddress(address);
			var client = new ServersListMaker.ServersListMakerClient(channel);
			//TODO: вынести получение айпи в отдельный метод!!!
			var reply = await client.AddServerToListAsync(
							  new Server { Name = name, Ip = ip, Port = port });
			Debug.WriteLine(reply.IsSuccec);
		}

	}
}
