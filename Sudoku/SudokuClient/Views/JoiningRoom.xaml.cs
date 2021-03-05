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
			//var ip = Dns.GetHostEntry(hostName).AddressList[1];
			//var port = UtilsLibrary.NetworkUtils.GetFreePort();
			//var port = 11000;
			var ipport = tbip.Text.Split(":");
			var ipS = ipport[0];
			var portS = ipport[1];
			var ip = IPAddress.Parse(ipS);
			var port = int.Parse(portS);
			var client = new SocketClient(ip, port);
			var game = new SudokuField(client);
			game.Show();
			Close();
		}
	}
}
