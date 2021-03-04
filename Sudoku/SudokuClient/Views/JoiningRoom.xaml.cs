using System.Net;
using System.Windows;
using UtilsLibrary.Servers;

namespace SudokuClient.Views
{
	/// <summary>
	/// Логика взаимодействия для ConnectingToGame.xaml
	/// </summary>
	public partial class JoiningRoom : Window
	{
		public JoiningRoom()
		{
			InitializeComponent();
		}

		private void JoinToGame(object sender, RoutedEventArgs e)
		{
			var hostName = Dns.GetHostName();
			var ip = Dns.GetHostEntry(hostName).AddressList[1];
			//var port = UtilsLibrary.NetworkUtils.GetFreePort();
			var port = 11000;

			var client = new SocketClient(ip, port);
			var game = new SudokuField(client);
			game.Show();
			Close();
		}
	}
}
