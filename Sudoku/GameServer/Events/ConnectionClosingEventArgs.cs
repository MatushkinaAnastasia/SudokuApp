using System;

namespace GameServer.Events
{
    public class ConnectionClosingEventArgs : EventArgs
    {
        public ConnectionClosingEventArgs(Client client)
        {
            Client = client;
        }

        public Client Client { get; }
    }
}
