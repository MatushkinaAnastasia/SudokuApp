using System.Windows;

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
			var game = new SudokuField();
			game.Show();
			Close();
		}
	}
}
