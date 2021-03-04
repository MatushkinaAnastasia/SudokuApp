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
            var socket = GameServerComm.RunGameServer();
			var game = new SudokuField(socket);
			game.Show();
			Close();
		}
    }
}
