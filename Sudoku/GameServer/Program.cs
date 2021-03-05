using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UtilsLibrary;
using UtilsLibrary.Data;
using UtilsLibrary.Servers;

namespace GameServer
{
	class Program : IMessageHandler
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly IPAddress _ip;
		private readonly int _port;

		private readonly string _connectionString;
		private const int _size = 9;
		private SudokuCell[,] _data;

		private List<Client> _clients; //кункурентный лист сделать.

		public Program(string ip, string port)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_ip = IPAddress.Parse(ip);
			_port = int.Parse(port);
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
			var program = new Program(args[0], args[1]);
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
			var command = message[0];

			switch (command)
			{
				case 0:
					{
						// send table to client
						Console.WriteLine("Отправка таблицы подключившемуся клиенту");
						Console.WriteLine($"Длина таблицы {_data.Length}"); ;
						var ipB = new byte[4];
						Array.Copy(message, 1, ipB, 0, 4);
						var portB = new byte[4];
						Array.Copy(message, 5, portB, 0, 4);

						var ip = new IPAddress(ipB);
						var port = BitConverter.ToInt32(portB, 0);

						_clients.Add(new Client(ip, port));

						socket.Send(_data.ConvertToByteArray());
						break;
					}
				case 1:
					{
						var x = message[1];
						var y = message[2];
						var value = message[3];
						if (value != 0) //проверка на пустое значение!
						{
							_data[x, y].Value = value;
							Console.WriteLine("Получено значение:");
							Console.WriteLine($"x: {x} y: {y} value: {value}");
							SendMessageToAllClients(x, y, value);
						}

						break;
					}

				case 2:
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
