using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerGRPC
{
	public class ServersListMakerService : ServersListMaker.ServersListMakerBase
	{
		private readonly object _locker;
		private List<Server> _servers;
		public ServersListMakerService()
		{
			_locker = new object();
			_servers = new List<Server>();
		}

		public override Task<IsSuccecfull> AddServerToList(Server request, ServerCallContext context)
		{
			Console.WriteLine("AddServerToList");
			try
			{
				var isSucces = new IsSuccecfull { IsSuccec = "1" };
				lock (_locker)
				{
					_servers.Add(request);
					Console.WriteLine($"Добавленный сервер: {request.Name} Находится по адресу: {request.Ip}:{request.Port}");
				}
				return Task.FromResult(isSucces);
			}
			catch (Exception e)
			{
				Console.WriteLine($"ВОЗНИКЛА ПРОБЛЕМА:{e}");
				var isSucces = new IsSuccecfull { IsSuccec = "0" };
				return Task.FromResult(isSucces);
			}
		}

		public override Task<Servers> ReturnServersList(RequestFromClient request, ServerCallContext context)
		{
			var result = new Servers();
			result.Servers_.Add(_servers);
			return Task.FromResult(result);
		}

		public override Task<IsSuccecfull> DeleteServerFromList(Server request, ServerCallContext context)
		{
			Console.WriteLine($"Удаление:");
			try
			{
				Console.WriteLine("Список серверов");
				foreach (var server in _servers)
				{
					Console.WriteLine($"server: {server.Name} Находился по адресу: {server.Ip}:{server.Port}");
				}
				lock (_locker)
				{
					var server = _servers.FirstOrDefault(x => x.Name == request.Name && x.Ip == request.Ip && x.Port == request.Port);
					if (server != null)
					{
						var res = _servers.Remove(server);
						Console.WriteLine($"Результат удаления: { res }");
					}

					Console.WriteLine($"request: {request.Name} Находился по адресу: {request.Ip}:{request.Port}");
					Console.WriteLine($"server: {server?.Name} Находился по адресу: {server?.Ip}:{server?.Port}");
				}

				Console.WriteLine("Список серверов");
				foreach (var server in _servers)
				{
					Console.WriteLine($"server: {server.Name} Находился по адресу: {server.Ip}:{server.Port}");
				}

				var isSucces = new IsSuccecfull { IsSuccec = "1" };
				return Task.FromResult(isSucces);
			}
			catch (Exception e)
			{
				Console.WriteLine($"ВОЗНИКЛА ПРОБЛЕМА:{e}");
				var isSucces = new IsSuccecfull { IsSuccec = "0" };
				return Task.FromResult(isSucces);
			}
		}
	}
}