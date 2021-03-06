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
		public JoiningRoom()
		{
			InitializeComponent();
			DataContext = this;
		}

		public ObservableCollection<Server> Servers { get; private set; }
		public Server SelectedServer { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void JoinToGame(object sender, RoutedEventArgs e)
		{
			if (SelectedServer == null)
			{
				MessageBox.Show("Выберите, пожалуйста, комнату :)");
				return;
			}
			var ip = IPAddress.Parse(SelectedServer.Ip);
			var port = int.Parse(SelectedServer.Port);
			var client = new SocketClient(ip, port);
			var game = new SudokuField(client);
			game.Show();
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
				var servers = await ClientGrpc.GetServers();
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
	}
}
