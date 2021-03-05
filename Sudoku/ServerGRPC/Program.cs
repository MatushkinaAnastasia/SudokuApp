using System;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using System.Configuration;
using System.Threading.Tasks;
using System.Net;

namespace ServerGRPC
{
	class Program
	{
		public static async Task Main(string[] args)
		{
			var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1];
			int port = int.Parse(ConfigurationManager.AppSettings.Get("port"));

			var reflectionServiceImpl = new ReflectionServiceImpl(ServersListMaker.Descriptor, ServerReflection.Descriptor);

			Grpc.Core.Server server = new Grpc.Core.Server
			{
				Services =
				{
					ServersListMaker.BindService(new ServersListMakerService()), //для сервиса устанвливаем обработчик
					ServerReflection.BindService(reflectionServiceImpl)
				},
				Ports = { new ServerPort(ip.ToString(), port, ServerCredentials.Insecure) }
			};
			server.Start();

			Console.WriteLine($"Сервер запущен по адресу {ip}:{port}");
			Console.WriteLine("Нажмите любую клавишу для выхода");
			Console.ReadKey();
			Console.WriteLine("Сервер завершает работу");
			await server.ShutdownAsync();
			Console.WriteLine("Сервер закрыт");
			Console.ReadKey();
		}
	}
}
