using Microsoft.Win32;
using SudokuClient.GameUtils;
using System;
using System.Windows;
using System.Windows.Media;
using UtilsLibrary.Grpc;

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

		private void CreateRoom(object sender, RoutedEventArgs e)
		{
			StartGame(nameOfRoom.Text);
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
			if (nameOfRoom.Length > 30)
			{
				MessageBox.Show("Имя комнаты не должно превышать 30 символов");
				return;
			}

			var gameServerWrapper = new GameServerWrapper(nameOfRoom, path);
			gameServerWrapper.Start();

			var grpc = new ClientGrpc();
			var sudokuWindow = new SudokuWindow(gameServerWrapper.IPEndPoint);

			sudokuWindow.Loaded += async (s, e) =>
			{
				try
				{
					await grpc.SendRoom(nameOfRoom, gameServerWrapper.IPEndPoint.Address.ToString(), gameServerWrapper.IPEndPoint.Port.ToString());
				}
				catch { /*ignore*/ }
			};

			sudokuWindow.Closed += async (s, e) =>
			{
				gameServerWrapper?.Stop();
				try
				{
					await grpc.DeleteServer(nameOfRoom, gameServerWrapper.IPEndPoint.Address.ToString(), gameServerWrapper.IPEndPoint.Port.ToString());
				}
				catch { /*ignore*/ }
				Close();
			};

			Hide();
			sudokuWindow.Show();
		}
	}
}
