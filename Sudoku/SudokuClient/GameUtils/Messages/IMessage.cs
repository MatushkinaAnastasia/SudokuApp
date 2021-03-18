namespace SudokuClient.GameUtils.Messages
{
    public interface IMessage
    {
        byte Id { get; }

        byte[] Serialize();
    }
}
