﻿using System;
using System.Windows;

namespace SudokuClient.Views
{
	public partial class Menu : Window
	{
		public Menu()
		{
			InitializeComponent();
			AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); //нешифрованные данные
		}

		private void CreateGame(object sender, RoutedEventArgs e)
		{
			var createRoom = new CreatingRoom();
			createRoom.Show();
			Close();
		}

		private void JoinToGame(object sender, RoutedEventArgs e)
		{
			var joinRoom = new JoiningRoom();
			joinRoom.Show();
			Close();
		}
		private void ShowRules(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Правила судоку на сайте sudoku.com. Удачи :)");
		}

		private void Exit(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OpenChat(object sender, RoutedEventArgs e)
		{
			var chat = new ChatWindow();
			chat.Show();
		}
	}
}
