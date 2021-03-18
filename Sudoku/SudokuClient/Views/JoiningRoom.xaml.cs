using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using UtilsLibrary.Grpc;

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
			IPEndPoint address;
			try
			{
				//Hide();
				address = IPEndPoint.Parse(tbip.Text.Trim());
			}
			catch
			{
				MessageBox.Show("Выберите, пожалуйста, комнату или введите корректный адрес:)");
				return;
			}

			var sudokuWindow = new SudokuWindow(address);
			sudokuWindow.Closing += (s, e) => Show();
			Hide();
			sudokuWindow.Show();
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
				MessageBox.Show("Потеряна связь с сервером. Введите адрес вручную :(");
			}
		}

		private void BackToMenu(object sender, RoutedEventArgs e)
		{
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
