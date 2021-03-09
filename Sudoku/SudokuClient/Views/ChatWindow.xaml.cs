using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
		public ChatWindow()
		{
			InitializeComponent();
			_server = new RabbitMQServer("localhost");
			_client = new RabbitMQClient("localHost");
			_cancellationTokenSource = new CancellationTokenSource();
			Task.Run(() => _server.Run(this, _cancellationTokenSource.Token));
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
	}
}
