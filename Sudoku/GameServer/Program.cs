using GameServer.Tools;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
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

		private DateTime _lastSave = DateTime.Now;

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
				case GameServerProtocol.Save:
					{
						if ((DateTime.Now - _lastSave) > TimeSpan.FromSeconds(5))
						{
							//TODO: try catch! return 1 and 0
							WriteDataToFile();
						}
						break;
					}
				case GameServerProtocol.Load:
					{
						var loadedData = LoadDataFromFile();
						socket.Send(loadedData);
					}
					break;
				case GameServerProtocol.Check:
					{
						Console.WriteLine("Проверка ответов");
						Console.WriteLine("Лог действий:");
						var answer = Check();
						Console.WriteLine($"Результат {answer}");
						socket.Send(Encoding.UTF8.GetBytes(answer));
						break;
					}
				default: break;
			}
		}

		private string Check()
		{
			var arr = new byte[9];
			Console.WriteLine("Массив строк");

			//проверка строк
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					arr[j] = _data[i, j].Value;

					Console.Write($" {arr[j]} ");
				}
				Console.WriteLine();
				Console.WriteLine($"Отправленный массив: {arr[0]} {arr[1]} {arr[2]} {arr[3]} {arr[4]} {arr[5]} {arr[6]} {arr[7]}");
				if (IsValid(arr) == false)
				{
					return "0";
				}
				arr = new byte[9];
			}

			//проверка столбцов
			Console.WriteLine("Массив столбцов");
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					arr[j] = _data[j, i].Value;

					Console.Write($" {arr[j]} ");
				}

				Console.WriteLine();
				if (IsValid(arr) == false)
				{
					return "0";
				}
				arr = new byte[9];
			}

			//проверка чисел в трешках
			Console.WriteLine("Ячейки в трешках");
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					var k = 0;
					{
						for (int a = 0; a < 3; a++)
						{
							for (int b = 0; b < 3; b++)
							{
								int x = i * 3 + a;
								int y = j * 3 + b;
								arr[k] = _data[x, y].Value;
								Console.Write($" {arr[k]} ");
								k++;
							}
						}
					}
					Console.WriteLine();
					if (IsValid(arr) == false)
					{
						return "0";
					}
					arr = new byte[9];
				}
			}
			return "1";

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
		private byte[] LoadDataFromFile()
		{
			using var streamReader = new StreamReader("save.sudoku");
			var data = streamReader.ReadToEnd();
			var byteData = Encoding.UTF8.GetBytes(data);
			_data = SudokuCellExtensions.ConvertToSudokuCellArray(byteData);

			if (byteData.Length != 0)
			{
				return byteData;
			}
			else
			{
				Console.WriteLine("Файл пустой!");
				return null;
			}
		}

		private void WriteDataToFile()
		{
			//using var streamWriter = new StreamWriter($"{_nameOfRoom}-{_lastSave.ToString().Replace(":", "_")}.sudoku");
			using var streamWriter = new StreamWriter("save.sudoku");
			var data = SudokuCellExtensions.ConvertToByteArray(_data);
			var stringData = Encoding.UTF8.GetString(data);

			streamWriter.Write(stringData);

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
