using SudokuClient.Tools;
using System.Threading.Tasks;
using System.Windows;

namespace SudokuClient.Views
{
	public partial class CreatingRoom : Window
    {
        public CreatingRoom()
        {
            InitializeComponent();
        }

        private async void CreateRoomAsync(object sender, RoutedEventArgs e)
        {
            var socket = GameServerComm.RunGameServer();
			var game = new SudokuField(socket);
            await UtilsLibrary.Grpc.ClientGrpc.SendRoom();
			game.Show();
			Close();
		}
    }
}
