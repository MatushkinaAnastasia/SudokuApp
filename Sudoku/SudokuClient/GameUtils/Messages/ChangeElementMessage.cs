namespace SudokuClient.GameUtils.Messages
{
    public class ChangeElementMessage : IMessage
    {
        public ChangeElementMessage(byte x, byte y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public byte Id => 0x00;

        public byte X { get; }

        public byte Y { get; }

        public byte Value { get; }

        public byte[] Serialize()
        {
            return new byte[] { X, Y, Value };
        }
    }
}
