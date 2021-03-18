using Grpc.Net.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace UtilsLibrary.Grpc
{
	public class ClientGrpc
	{
		private readonly string _ip;
		private readonly string _port;
		private readonly string _address;

		public ClientGrpc()
		{
			_ip = ConfigurationManager.AppSettings.Get("ipGrpc");
			_port = ConfigurationManager.AppSettings.Get("portGrpc");
			_address = $"http://{_ip}:{_port}";
		}

		public async Task<IsSuccecfull> SendRoom(string name, string ip, string port)
		{
			using var channel = GrpcChannel.ForAddress(_address);
			var client = new ServersListMaker.ServersListMakerClient(channel);
			var reply = await client.AddServerToListAsync(new Server { Name = name, Ip = ip, Port = port });

			return reply;
		}

		public async Task<List<Server>> GetServers()
		{
			using var channel = GrpcChannel.ForAddress(_address);
			var client = new ServersListMaker.ServersListMakerClient(channel);
			var reply = await client.ReturnServersListAsync(new RequestFromClient { ReqArg = "" });

			return reply.Servers_.ToList();
		}

		public async Task<IsSuccecfull> DeleteServer(string name, string ip, string port)
		{
			using var channel = GrpcChannel.ForAddress(_address);
			var client = new ServersListMaker.ServersListMakerClient(channel);
			var reply = await client.DeleteServerFromListAsync(new Server { Name = name, Ip = ip, Port = port });

			return reply;
		}
	}
}
