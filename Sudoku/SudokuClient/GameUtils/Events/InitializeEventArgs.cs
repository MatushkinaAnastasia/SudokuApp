using System.Text;
using UtilsLibrary.Data;

namespace SudokuClient.GameUtils.Events
{
    public class InitializeEventArgs
    {
        private readonly byte[] _data;
        private readonly byte[] _roomName;

        public InitializeEventArgs(byte[] data, byte[] roomName)
        {
            _data = data;
            _roomName = roomName;
        }

        public SudokuCell[,] Data
        {
            get => _data.ConvertToSudokuCellArray();
        }

        public string RoomName
        {
            get => Encoding.UTF8.GetString(_roomName);
        }
    }
}
