using SudokuClient.Tools;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SudokuClient.Views
{
	public partial class CreatingRoom : Window
    {
        public CreatingRoom()
        {
            InitializeComponent();
            nameOfRoom.GotFocus += RemoveText;
            nameOfRoom.LostFocus += AddText;
        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (nameOfRoom.Text == "Название комнаты")
            {
                nameOfRoom.Text = "";
                nameOfRoom.Foreground = Brushes.Black;
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameOfRoom.Text))
                nameOfRoom.Text = "Название комнаты";
            nameOfRoom.Foreground = Brushes.Gray;
        }

        private async void CreateRoomAsync(object sender, RoutedEventArgs e)
        {
            var socket = await GameServerComm.RunGameServerAsync(nameOfRoom.Text);
			var game = new SudokuField(socket);
			game.Show();
			Close();
		}

        
    }
}
