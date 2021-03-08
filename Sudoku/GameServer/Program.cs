using GameServer.Tools;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilsLibrary;
using UtilsLibrary.Data;
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

		private readonly string _connectionString;
		private const int _size = 9;
		private SudokuCell[,] _data;

		private List<Client> _clients; //TODO: кункурентный лист сделать.

		public Program(string ip, string port, string nameOfRoom)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_ip = IPAddress.Parse(ip);
			_port = int.Parse(port);
			_nameOfRoom = nameOfRoom.Replace("_", " ");
			_clients = new List<Client>();

			var path = PathWorker.GetPath("pathToDB");
			_connectionString = new SQLiteConnectionStringBuilder
			{
				DataSource = path,
			}.ConnectionString;
			_data = GetData();
		}

		#region lol
		private static void Main(string[] args)
		{
			var program = new Program(args[0], args[1], args[2]);
			//var program = new Program("192.168.1.50", "11000");
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
		}

		public void Run()
		{
			IServer server = new SocketServer(_ip, _port);
			var task = Task.Run(() => server.Run(this, _cancellationTokenSource.Token));

			Console.WriteLine("Сервер находится по адресу:" + _ip + ":" + _port);
			Console.WriteLine("Сервер в режиме ожидания");

			Task.WaitAll(task);
		}

		public void Cancel()
		{
			_cancellationTokenSource.Cancel();
		}
		#endregion

		//работа с полученными данными
		public void Handle(byte[] message, Socket socket)
		{
			var command = (GameServerProtocol)message[0];

			switch (command)
			{
				case GameServerProtocol.Init:
					{
						var client = GameServerProtocolWorker.GetClient(message, 1);
						_clients.Add(client);

						Console.WriteLine("Отправка таблицы подключившемуся клиенту");
						var bytes = GameServerProtocolWorker.GetDataAndNameBytes(_data, _nameOfRoom);
						socket.Send(bytes);
						break;
					}
				case GameServerProtocol.SetValue:
					{
						(var x, var y, var value) = GameServerProtocolWorker.GetXYValue(message, 1);
						
						if (value != 0) //проверка на пустое значение!
						{
							_data[x, y].Value = value;
							Console.WriteLine("Получено значение:");
							Console.WriteLine($"x: {x} y: {y} value: {value}");
							SendMessageToAllClients(x, y, value);
						}

						break;
					}
				case GameServerProtocol.Disconnect:
					{
						break;
					}
				default: break;
			}
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

		private SudokuCell[,] GetData() // TODO: вынести модуль с бд.
		{
			using var conn = new SQLiteConnection(_connectionString);
			var sCommand = new SQLiteCommand()
			{
				Connection = conn,
				CommandText = @"SELECT numbers FROM sudoku_data ORDER BY RANDOM() LIMIT 1;"
			};

			conn.Open();
			var numbers = (string)sCommand.ExecuteScalar();

			string[] nine_lines = numbers.Split('!');

			var data = new SudokuCell[_size, _size];
			for (byte i = 0; i < _size; i++)
			{
				var t = nine_lines[i];
				for (byte j = 0; j < _size; j++)
				{
					var symbol = t[j];
					SudokuCell cell;

					if (symbol != '*')
					{
						cell = new SudokuCell(i, j, byte.Parse(symbol.ToString()), true);
					}
					else
					{
						cell = new SudokuCell(i, j, 0, false);
					}

					data[i, j] = cell;
				}

			}
			return data;
		}
	}
}
