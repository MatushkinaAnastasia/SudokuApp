using SudokuClient.Tools;
using System;
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

		private async void CreateRoomAsync(object sender, RoutedEventArgs e)
		{
			SocketClient socket = null;

			try
			{
				socket = await GameServerComm.RunGameServerAsync(nameOfRoom.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			var game = new SudokuField(socket);
			game.Show();
			_isGameStarted = true;
			Close();
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!_isGameStarted)
			{
				var menu = new Menu();
				menu.Show();
			}
		}
	}
}
