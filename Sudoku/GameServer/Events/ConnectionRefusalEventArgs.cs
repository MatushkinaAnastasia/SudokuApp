using System;

namespace GameServer.Events
{
    public class ConnectionRefusalEventArgs : EventArgs
    {
        public ConnectionRefusalEventArgs(Client client)
        {
            Client = client;
        }

        public Client Client { get; }
    }
}
