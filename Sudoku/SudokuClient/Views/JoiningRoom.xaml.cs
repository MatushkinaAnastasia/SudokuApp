using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using UtilsLibrary.Grpc;
using UtilsLibrary.Servers;

namespace SudokuClient.Views
{
	/// <summary>
	/// Логика взаимодействия для ConnectingToGame.xaml
	/// </summary>
	public partial class JoiningRoom : Window, INotifyPropertyChanged
	{
		private readonly ClientGrpc _clientGrpc;

		public JoiningRoom()
		{
			InitializeComponent();
			_clientGrpc = new ClientGrpc();
			tbip.Text = string.Empty;

			DataContext = this;
		}

		public ObservableCollection<Server> Servers { get; private set; }
		public Server SelectedServer { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void JoinToGame(object sender, RoutedEventArgs e)
		{
			//todo: сделать timeout
			IPAddress ip;
			int port;
			try
			{
				var slices = tbip.Text.Split(":");
				ip = IPAddress.Parse(slices[0]);
				port = int.Parse(slices[1]);
				if (port < 1 || port > 65535)
				{
					throw new System.Exception();
				}
			}
			catch
			{
				MessageBox.Show("Выберите, пожалуйста, комнату или введите корректный адрес:)");
				return;
			}
			
			var client = new SocketClient(ip, port);
			try
			{
				var game = new SudokuField(client);
				game.Show();
			} 
			catch
			{
				MessageBox.Show("Кажется, такой комнаты уже нет :(");
				return;
			}
			Close();
		}

		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			 RefreshTable(null, null);
		}

		private async void RefreshTable(object sender, RoutedEventArgs e)
		{
			try
			{
				var servers = await _clientGrpc.GetServers();
				Servers = new ObservableCollection<Server>(servers);
			} 
			catch
			{
				Servers = new ObservableCollection<Server>();
				MessageBox.Show("Потеряна связь с сервером :(");
			}
		}

		private void BackToMenu(object sender, RoutedEventArgs e)
		{
			var menu = new Menu();
			menu.Show();
			Close();
		}

		private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (SelectedServer != null)
			{
				tbip.Text = $"{SelectedServer.Ip}:{SelectedServer.Port}";
			}
		}
	}
}
