using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SudokuClient.Views
{
	/// <summary>
	/// Логика взаимодействия для ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window
	{
		public ChatWindow()
		{
			InitializeComponent();
		}

		private void SendMessage(object sender, RoutedEventArgs e)
		{
			messagesBox.Text += "— " + enterBox.Text + '\n';
			enterBox.Text = "";
		}
	}
}
