using System.IO;

namespace SudokuClient.GameUtils.Events
{
    public class ConnectionRefusedEventArgs
    {
        public ConnectionRefusedEventArgs(IOException cause)
        {
            Cause = cause;
        }

        public IOException Cause;
    }
}
