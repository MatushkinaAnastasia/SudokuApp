using Microsoft.Win32;
using SudokuClient.Tools;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using UtilsLibrary.Servers;

namespace SudokuClient.Views
{
	public partial class CreatingRoom : Window
	{
		public CreatingRoom()
		{
			InitializeComponent();
			nameOfRoom.GotFocus += RemoveText;
			nameOfRoom.LostFocus += AddText;
		}

		~CreatingRoom()
		{
			nameOfRoom.GotFocus -= RemoveText;
			nameOfRoom.LostFocus -= AddText;
		}

		public void RemoveText(object sender, EventArgs e)
		{
			if (nameOfRoom.Text == "Название комнаты")
			{
				nameOfRoom.Text = "";
				nameOfRoom.Foreground = Brushes.Black;
			}
		}

		public void AddText(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(nameOfRoom.Text))
				nameOfRoom.Text = "Название комнаты";
			nameOfRoom.Foreground = Brushes.Gray;
		}

		private bool _isGameStarted = false;

		private void CreateRoom(object sender, RoutedEventArgs e)
		{
			StartGame(nameOfRoom.Text);
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!_isGameStarted)
			{
				var menu = new Menu();
				menu.Show();
			}
		}

		private void LoadGame(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Filter = "Sudoku file (*.sudoku)|*.sudoku"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string path = openFileDialog.FileName;
				StartGame(nameOfRoom.Text, path);
			}
		}

		private void StartGame(string nameOfRoom, string path = "")
		{
			SocketClient socket;
			try
			{
				socket = GameServerComm.RunGameServer(nameOfRoom, path);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Проблемы создания комнаты. Проверьте путь до gameserver.exe в конфигурационном файле.");
				Console.WriteLine(ex.Message);
				return;
			}

			var game = new SudokuField(socket);
			game.Show();
			_isGameStarted = true;
			Close();
		}
	}
}
