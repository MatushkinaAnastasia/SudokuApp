namespace SudokuClient.GameUtils.Events
{
    public class ElementChangedEventArgs
    {
        public ElementChangedEventArgs(int x, int y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public int X { get; }

        public int Y { get; }

        public byte Value { get; }
    }
}
