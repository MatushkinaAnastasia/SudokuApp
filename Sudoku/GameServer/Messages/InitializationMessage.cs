using System;
using System.Linq;
using System.Text;
using UtilsLibrary.Data;

namespace GameServer.Messages
{
    public class InitializationMessage : IMessage
    {
        private readonly SudokuCell[,] _data;
        private readonly string _roomName;

        public InitializationMessage(SudokuCell[,] data, string roomName)
        {
            _data = data;
            _roomName = roomName;
        }

        public byte GetId()
        {
            return 0;
        }

        public byte[] Serialize()
        {
            var data = _data.ConvertToByteArray();
            var roomName = Encoding.UTF8.GetBytes(_roomName);
            var length = BitConverter.GetBytes(roomName.Length);

            return data.Concat(length).Concat(roomName).ToArray();
        }
    }
}
