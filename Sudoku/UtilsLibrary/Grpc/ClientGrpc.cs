using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Grpc.Net.Client;

namespace UtilsLibrary.Grpc
{
	public class ClientGrpc
	{
		public static async Task SendRoom()
		{
			//TODO: убрать статический порт
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
			var address = $"http://{Dns.GetHostEntry(Dns.GetHostName()).AddressList[1]}:5000";
			using var channel = GrpcChannel.ForAddress(address);
			var client = new ServersListMaker.ServersListMakerClient(channel);
			//TODO: вынести получение айпи в отдельный метод
			var reply = await client.AddServerToListAsync(
							  new Server { Name = "Комната1", Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString(), Port = "5000" });
			Console.WriteLine(reply.IsSuccec);
		}

	}
}
