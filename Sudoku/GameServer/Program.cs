using GameServer.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLibrary;
using UtilsLibrary.Data;
using UtilsLibrary.Grpc;
using UtilsLibrary.Servers;
using UtilsLibrary.Tools;

namespace GameServer
{
	class Program : IMessageHandler
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly IPAddress _ip;
		private readonly int _port;
		private readonly string _nameOfRoom;
		private readonly string _pathToFile;

		private readonly ClientGrpc _clientGrpc;

		private SudokuCell[,] _data;

		private List<Client> _clients;

		public Program(IPAddress ip, int port, string nameOfRoom, string pathToFile)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_ip = ip;
			_port = port;
			_nameOfRoom = nameOfRoom;
			_pathToFile = pathToFile;
			_clients = new List<Client>();
			_clientGrpc = new ClientGrpc();


			string path = "";
			try
			{
				path = PathWorker.GetPath("pathToDB");
			}
			catch
			{
				Console.WriteLine("Неверный путь до базы данных. Скорректируйте его в конфигурационном файле и перезапустите.");
				Console.ReadKey();
				return;
			}
			var db = new DatabaseWorker(path);

			Console.WriteLine(_pathToFile);
			if (string.IsNullOrEmpty(_pathToFile))
			{
				_data = db.GetData();
			}
			else
			{
				Console.WriteLine("errors?");
				_data = LoadDataFromFile(_pathToFile);
				Console.WriteLine("no errors");
			}
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); //нешифрованные данные
		}

		// entry point of program
		private static void Main(string[] args)
		{
			IPAddress ip = NetworkUtils.GetMyIp();
			int port = 1000;
			string nameOfRoom = "noName";
			string pathToFile = null;
			try
			{
				ip = IPAddress.Parse(args[0]);
				port = int.Parse(args[1]);
				nameOfRoom = args[2].Replace("_", " ");

				if (args.Length == 4)
				{
					pathToFile = args[3].Replace("?", " ");
				}

			}
			catch { }
			var program = new Program(ip, port, nameOfRoom, pathToFile);

			Console.CancelKeyPress += (sender, e) =>
			{
				try
				{
					program.Cancel();
				}
				finally
				{
					e.Cancel = true;
				}
			};
			program.Run();
			Console.WriteLine("PRESS ANY KEY TO EXIT");
			Console.ReadKey();
		}

		public void Run()
		{
			IServer server = new SocketServer(_ip, _port);
			var task = Task.Run(() => server.Run(this, _cancellationTokenSource.Token));

			Console.WriteLine("Сервер находится по адресу:" + _ip + ":" + _port);
			Console.WriteLine("Сервер в режиме ожидания");

			// say gRPC server to add me to list of servers
			Task.Run(async () =>
			{
				try
				{
					Console.WriteLine("SendRoom start...");
					var result = await _clientGrpc.SendRoom(_nameOfRoom, _ip.ToString(), _port.ToString());
					Console.WriteLine("SendRoom success.");
				}
				catch { Console.WriteLine("SendRoom error"); }
			});

			Task.WaitAll(task);
		}

		public void Cancel()
		{
			_cancellationTokenSource.Cancel();
		}

		//работа с полученными данными
		public void Handle(byte[] message, Socket socket)
		{
			var command = (GameServerProtocol)message[0];

			switch (command)
			{
				case GameServerProtocol.Connect:
					{
						var client = GameServerProtocolWorker.GetClient(message, 1);
						Console.WriteLine($"Клиент подключился. ip: {client.IP}, port: {client.Port}");
						_clients.Add(client);

						var bytes = GameServerProtocolWorker.GetDataAndNameBytes(_data, _nameOfRoom);
						Console.WriteLine("Отправка таблицы подключившемуся клиенту");
						socket.Send(bytes);
						Console.WriteLine("Таблица отправлена.");
						break;
					}
				case GameServerProtocol.SetValue:
					{
						(var x, var y, var value) = GameServerProtocolWorker.GetXYValue(message, 1);

						if (value < 10)
						{
							_data[x, y].Value = value;
							Console.WriteLine($"Получено значение - x: {x} y: {y} value: {value}");
							SendMessageToAllClients(x, y, value);
						}

						break;
					}
				case GameServerProtocol.Disconnect:
					{
						var client = GameServerProtocolWorker.GetClient(message, 1);
						var myClient = _clients.FirstOrDefault(x => x.IsEquals(client));
						if (myClient != null)
						{
							try
							{
								var reply = myClient.SocketClient.SendAndRecieve(new byte[] { 2 });
								if (reply[0] == 0)
								{
									break;
								}
							}
							catch { }
							finally
							{
								_clients.Remove(myClient);
								CheckClients();
							}
						}

						break;
					}
				case GameServerProtocol.Check:
					{
						Console.WriteLine("Проверка ответов");
						var answer = Check();
						Console.WriteLine($"Результат {answer}");
						socket.Send(new byte[] { answer ? (byte)1 : (byte)0 });
						break;
					}
				default: break;
			}
		}

		private async void CheckClients()
		{
			Console.WriteLine($"clients count: {_clients.Count}");

			if (_clients.Count == 0)
			{
				await Task.Run(async () =>
				{
					try
					{
						await _clientGrpc.DeleteServer(_nameOfRoom, _ip.ToString(), _port.ToString());
					}
					catch { }
				});

				Cancel();
			}
		}

		private bool Check()
		{
			var arr = new byte[9];

			//проверка строк
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					arr[j] = _data[i, j].Value;
				}
				if (IsValid(arr) == false)
				{
					return false;
				}
			}

			//проверка столбцов
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					arr[j] = _data[j, i].Value;
				}
				if (IsValid(arr) == false)
				{
					return false;
				}
			}

			//проверка чисел в трешках
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					var k = 0;
					for (int a = 0; a < 3; a++)
					{
						for (int b = 0; b < 3; b++)
						{
							int x = i * 3 + a;
							int y = j * 3 + b;
							arr[k] = _data[x, y].Value;
							k++;
						}
					}
					if (IsValid(arr) == false)
					{
						return false;
					}
				}
			}
			return true;
		}

		//проверяет повторяются ли числа в массиве
		private bool IsValid(byte[] arr)
		{
			Console.WriteLine($"Полученный массив: {arr[0]} {arr[1]} {arr[2]} {arr[3]} {arr[4]} {arr[5]} {arr[6]} {arr[7]}");
			for (int i = 0; i < arr.Length - 1; i++)
			{
				for (int j = i + 1; j < arr.Length; j++)
				{
					if (arr[i] == arr[j])
					{
						Console.WriteLine($"Элементы {arr[i]} и {arr[j]} не уникальны!");
						return false;
					}
				}
			}
			return true;
		}
		private SudokuCell[,] LoadDataFromFile(string path)
		{
			using var streamReader = new StreamReader(path);
			var data = streamReader.ReadToEnd();
			var byteData = Encoding.UTF8.GetBytes(data);

			if (byteData.Length == 0)
			{
				Console.WriteLine("Файл пустой!");
			}
			return SudokuCellExtensions.ConvertToSudokuCellArray(byteData);
		}

		private void SendMessageToAllClients(byte x, byte y, byte value)
		{
			foreach (var client in _clients)
			{
				try
				{
					client.SocketClient.Send(new byte[] { 1, x, y, value });
				}
				catch
				{
					// с клиентом потеряна связь.
					Console.WriteLine($"Клиент {client.IP}:{client.Port} отключился");
					_clients.Remove(client);
				}
			}
		}
	}
}
