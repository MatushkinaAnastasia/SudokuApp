using Grpc.Core;
using System;
using System.Collections.Generic;
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
			try
			{
				var isSucces = new IsSuccecfull { IsSuccec = "1" };
				lock (_locker)
				{
					_servers.Add(request);
					Console.WriteLine($"Добавленный сервер: {request.Name} Находится по адресу: {request.Ip}:{request.Port}");
				}
				return Task.FromResult(isSucces);
			} catch (Exception e)
			{
				Console.WriteLine($"ВОЗНИКЛА ПРОБЛЕМА:{e}");
				var isSucces = new IsSuccecfull { IsSuccec = "0" };
				return Task.FromResult(isSucces);
			}
		}
	}
}