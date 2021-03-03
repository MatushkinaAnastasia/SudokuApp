using SudokuClient.Tools;
using System.Windows;

namespace SudokuClient.Views
{
	public partial class CreatingRoom : Window
    {
        public CreatingRoom()
        {
            InitializeComponent();
        }

        private void CreateRoom(object sender, RoutedEventArgs e)
        {
            Communication.SendData();
			var game = new SudokuField();
			game.Show();
			Close();
		}
    }
}
