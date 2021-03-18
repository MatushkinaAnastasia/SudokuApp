using System;
using System.Net.Sockets;

namespace SudokuClient.GameUtils.Events
{
    public class ConnectionFailedEventArgs
    {
        public ConnectionFailedEventArgs(Exception cause)
        {
            Cause = cause;
        }

        public Exception Cause { get; }
    }
}
