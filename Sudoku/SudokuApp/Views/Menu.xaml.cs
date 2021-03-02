using System;
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
	public partial class Menu : Window
	{
		public Menu()
		{
			InitializeComponent();
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

		private void SingleGame(object sender, RoutedEventArgs e)
		{
			var game = new SudokuField();
			game.Show();
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
	}
}
