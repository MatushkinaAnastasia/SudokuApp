using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UtilsLibrary.RabbitMQ;

namespace SudokuClient.Views
{
	/// <summary>
	/// Логика взаимодействия для ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window, IMessageHandler
	{
		private readonly RabbitMQServer _server;
		private readonly RabbitMQClient _client;
		private CancellationTokenSource _cancellationTokenSource;

		private string _nickName = "";
		public ChatWindow()
		{
			InitializeComponent();

			nickname.GotFocus += RemoveText;
			nickname.LostFocus += AddText;

			var hostName = ConfigurationManager.AppSettings.Get("Ip_MQ");
			_server = new RabbitMQServer(hostName);
			_client = new RabbitMQClient(hostName);
			_cancellationTokenSource = new CancellationTokenSource();
			try
			{
				Task.Run(() => _server.Run(this, _cancellationTokenSource.Token));
			}
			catch { }
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


		private bool _isConnectionExists = true;
		private void SendMessage(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(enterBox.Text) && !string.IsNullOrEmpty(_nickName))
			{
				try
				{
					_client.Send(enterBox.Text, _nickName);
					if (!_isConnectionExists)
					{
						messagesBox.Text += "[соединение с сервером чата восстановлено]\n";

						_cancellationTokenSource.Cancel();
						_cancellationTokenSource.Dispose();
						_cancellationTokenSource = new CancellationTokenSource();
						Task.Run(() => _server.Run(this, _cancellationTokenSource.Token));
					}
					_isConnectionExists = true;
				}
				catch
				{
					messagesBox.Text += "[нет соединения с сервером чата]\n";
					_isConnectionExists = false;
				}
				enterBox.Text = "";
			}
			else
			{
				MessageBox.Show("Введите, пожалуйста, никнейм.");
			}
		}

		public void Handle(string message)
		{
			var arr = message.Split("`");

			Dispatcher.Invoke(() => messagesBox.Text += $"({arr[1]}): {arr[0]} \n");
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

		private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				SendMessage(null, null);
			}
		}
	}
}
