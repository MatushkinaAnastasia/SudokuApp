using SudokuClient.Tools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UtilsLibrary.Data;
using UtilsLibrary.Servers;

namespace SudokuClient.Views
{
	public partial class SudokuField : Window, IMessageHandler
	{
		private const int _size = 9;
		private SudokuCell[,] _data;
		private TextBox[,] _tbs;
		private readonly SocketClient _socketClient;

		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly SocketServer _socketServer;
		private readonly IPAddress _ip;
		private readonly int _port;

		public SudokuField(SocketClient client)
		{
			InitializeComponent();

			_cancellationTokenSource = new CancellationTokenSource();
			_ip = UtilsLibrary.NetworkUtils.GetMyIp();
			_port = UtilsLibrary.NetworkUtils.GetFreePort();
			_socketServer = new SocketServer(_ip, _port);
			_socketClient = client;

			Task.Run(() => _socketServer.Run(this, _cancellationTokenSource.Token));
		}

		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			var ipBytes = _ip.GetAddressBytes();
			var portBytes = BitConverter.GetBytes(_port);
			var bytes = _socketClient.SendAndRecieve(new byte[] 
			{
				0, 
				ipBytes[0], ipBytes[1], ipBytes[2], ipBytes[3],
				portBytes[0], portBytes[1], portBytes[2], portBytes[3],
			});
			_data = bytes.ConvertToSudokuCellArray();
			_tbs = GridCreator.CreateGrid(grid, ValueChanging);

			for (int i = 0; i < _size; i++)
			{
				for (int j = 0; j < _size; j++)
				{
					var tb = _tbs[i, j];

					SudokuCell cell = _data[i, j];
					if (cell.Value == 0)
					{
						tb.Text = string.Empty;
					}
					else if (cell.IsPermanent)
					{
						tb.Text = cell.Value.ToString();
						tb.IsEnabled = false;
						tb.Foreground = Brushes.Black;
						tb.FontWeight = FontWeights.UltraBold;
					}
					else if (cell.Value > 0 && cell.Value <= 9)
					{
						tb.Text = cell.Value.ToString();
					}
					else
					{
						tb.Text = string.Empty;
					}
				}
			}
		}



		private readonly Dictionary<Key, byte> _keys = new Dictionary<Key, byte>()
		{
			{ Key.D1, 1 },
			{ Key.D2, 2 },
			{ Key.D3, 3 },
			{ Key.D4, 4 },
			{ Key.D5, 5 },
			{ Key.D6, 6 },
			{ Key.D7, 7 },
			{ Key.D8, 8 },
			{ Key.D9, 9 },
			{ Key.NumPad1, 1 },
			{ Key.NumPad2, 2 },
			{ Key.NumPad3, 3 },
			{ Key.NumPad4, 4 },
			{ Key.NumPad5, 5 },
			{ Key.NumPad6, 6 },
			{ Key.NumPad7, 7 },
			{ Key.NumPad8, 8 },
			{ Key.NumPad9, 9 },
		};

		private void ValueChanging(TextBox textbox, KeyEventArgs e)
		{
			if (e.Key == Key.Back)
			{
				// remove value;
				// socket set 0;
				return;
			}

			if ((int)e.Key >= (int)Key.D1 && (int)e.Key <= (int)Key.D9
				|| (int)e.Key >= (int)Key.NumPad1 && (int)e.Key <= (int)Key.NumPad9)
			{
				var parameters = textbox.Tag as TextBoxParametres;
				var value = _keys[e.Key];
				_socketClient.Send(new byte[] { 1, (byte)parameters.X, (byte)parameters.Y, value });
				e.Handled = true;
			}
			else
			{
				e.Handled = true;
			}
		}

		private void BackToMenu(object sender, RoutedEventArgs e)
		{
			var menu = new Menu();
			menu.Show();
			Close();
			//TODO: сообщтить серверу о выходе.
		}

		public void Handle(byte[] message, Socket socket)
		{
			switch (message[0])
			{
				case 0:
					{
						// send table to client
						//Console.WriteLine("Отправка таблицы подключившемуся клиенту");
						//Console.WriteLine($"Длина таблицы {_data.Length}"); ;
						//socket.Send(_data.ConvertToByteArray());
						break;
					}
				case 1:
					{
						var x = message[1];
						var y = message[2];
						var value = message[3];
						_data[x, y].Value = value;

						Dispatcher.Invoke(() =>
						{
							if (value == 0)
							{
								_tbs[x, y].Text = string.Empty;
							}
							else
							{
								_tbs[x, y].Text = value.ToString();
							}
						});

						break;
					}
				case 2:
					{
						break;
					}
				default: break;
			}
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_cancellationTokenSource.Cancel();
		}
	}
}

