using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace UtilsLibrary.Grpc
{
	public class ClientGrpc
	{
		public static async Task<IsSuccecfull> SendRoom(string name, string ip, string port)
		{
			//TODO: читать ip/порт из конфига
			var address = $"http://{NetworkUtils.GetMyIp()}:5000"; //работает если grpc сервер запускается на том же компе!
			using var channel = GrpcChannel.ForAddress(address);
			var client = new ServersListMaker.ServersListMakerClient(channel);

			System.Console.WriteLine($"шлем на {address} name: {name}, ip: {ip}, port: {port}");
			var reply = await client.AddServerToListAsync(new Server { Name = name, Ip = ip, Port = port });

			return reply;
		}

		public static async Task<List<Server>> GetServers()
		{
			var address = $"http://{NetworkUtils.GetMyIp()}:5000"; //работает если grpc сервер запускается на том же компе!
			using var channel = GrpcChannel.ForAddress(address);
			var client = new ServersListMaker.ServersListMakerClient(channel);
			var reply = await client.ReturnServersListAsync(
							  new RequestFromClient { ReqArg = ""});
			return reply.Servers_.ToList();
		}

		public static async Task<IsSuccecfull> DeleteServer(string name, string ip, string port)
		{
			var address = $"http://{NetworkUtils.GetMyIp()}:5000"; //работает если grpc сервер запускается на том же компе!
			using var channel = GrpcChannel.ForAddress(address);
			var client = new ServersListMaker.ServersListMakerClient(channel);
			var reply = await client.DeleteServerFromListAsync(
							  new Server { Name = name, Ip = ip, Port = port });

			return reply;
		}
	}
}
