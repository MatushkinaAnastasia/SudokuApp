using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			//TODO: читать ip/порт из конфига
			var address = $"http://{NetworkUtils.GetMyIp()}:5000"; //работает если grpc сервер запускается на том же компе!
			using var channel = GrpcChannel.ForAddress(address);
			var client = new ServersListMaker.ServersListMakerClient(channel);

			var reply = await client.AddServerToListAsync(
							  new Server { Name = name, Ip = ip, Port = port });
			Debug.WriteLine(reply.IsSuccec);
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
	}
}
