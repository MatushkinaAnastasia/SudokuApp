﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SudokuApp.Views
{
	/// <summary>
	/// Логика взаимодействия для CreatingRoom.xaml
	/// </summary>
	public partial class CreatingRoom : Window
	{
		public CreatingRoom()
		{
			InitializeComponent();
		}

		private void CreateRoom(object sender, RoutedEventArgs e)
		{
			var game = new SudokuField();
			game.Show();
			Close();
		}
	}
}
