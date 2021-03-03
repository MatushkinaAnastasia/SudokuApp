using System;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using System.Configuration;
using System.Threading.Tasks;

namespace ServerGRPC
{
	class Program
	{
		public static void Main(string[] args)
		{
			//var ip = ConfigurationManager.AppSettings.Get("IP");
			//int port = int.Parse(ConfigurationManager.AppSettings.Get("port"));

			//var reflectionServiceImpl = new ReflectionServiceImpl(GradeCalculation.Descriptor, ServerReflection.Descriptor);

			//Server server = new Grpc.Core.Server
			//{
			//	Services =
			//	{
			//		GradeCalculation.BindService(new GradeCalculationService()), //для сервиса устанвливаем обработчик
			//		ServerReflection.BindService(reflectionServiceImpl)
			//	},
			//	Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
			//};
			//server.Start();

			//Console.WriteLine($"[server is running on {ip}:{port}]");
			//Console.WriteLine("[press any key to exit]");
			//Console.ReadKey();
			//Console.WriteLine("[server is closing...]");
			//await server.ShutdownAsync();
			//Console.WriteLine("[server is closed]");
			//Console.ReadKey();
		}
	}
}
