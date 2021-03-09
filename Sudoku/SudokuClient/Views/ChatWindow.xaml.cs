using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UtilsLibrary;
using UtilsLibrary.RabbitMQ;

namespace SudokuClient.Views
{
	/// <summary>
	/// Логика взаимодействия для ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window, IMessageHandler
	{
		private RabbitMQServer _server;
		private RabbitMQClient _client;
		private CancellationTokenSource _cancellationTokenSource;

		private string _nickName = "";
		public ChatWindow()
		{
			InitializeComponent();

			nickname.GotFocus += RemoveText;
			nickname.LostFocus += AddText;

			var hostName = NetworkUtils.GetMyIp().ToString();
			//TODO: брать айпи из конфига
			_server = new RabbitMQServer(hostName);
			_client = new RabbitMQClient(hostName);
			_cancellationTokenSource = new CancellationTokenSource();
			Task.Run(() => _server.Run(this, _cancellationTokenSource.Token));
		}

		private void RemoveText(object sender, RoutedEventArgs e)
		{
			if (nickname.Text == "Ваш никнейм для чата")
			{
				nickname.Text = "";
				nickname.Foreground = Brushes.Black;
			}
		}

		private void AddText(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(nickname.Text))
				nickname.Text = "Ваш никнейм для чата";
			nickname.Foreground = Brushes.Gray;
		}

		~ChatWindow()
		{
			nickname.GotFocus -= RemoveText;
			nickname.LostFocus -= AddText;
		}

		private void SendMessage(object sender, RoutedEventArgs e)
		{
			_client.Send(enterBox.Text);
			enterBox.Text = "";
		}

		public void Handle(string message)
		{
			Dispatcher.Invoke(() => messagesBox.Text += "— " + message + '\n');
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_cancellationTokenSource.Cancel();
		}

		private void ApplyNickname(object sender, RoutedEventArgs e)
		{
			_nickName = nickname.Text;
			MessageBox.Show($"Ваш никнейм {_nickName}");
			nickname.Foreground = Brushes.Black;
		}
	}
}
