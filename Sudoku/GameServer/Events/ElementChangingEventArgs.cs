using System;

namespace GameServer.Events
{
    public class ElementChangingEventArgs : EventArgs
    {
        public ElementChangingEventArgs(byte x, byte y, byte value, Client client)
        {
            X = x;
            Y = y;
            Value = value;
            Client = client;
        }

        public byte X { get; }
        public byte Y { get; }
        public byte Value { get; }
        public Client Client { get; }

        public static ElementChangingEventArgs Deserialize(Client sender, byte[] bytes)
        {
            byte x = bytes[0];
            byte y = bytes[1];
            byte value = bytes[2];
            return new ElementChangingEventArgs(x, y, value, sender);
        }
    }
}
