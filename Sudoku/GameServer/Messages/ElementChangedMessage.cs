using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Messages
{
    public class ElementChangedMessage : IMessage
    {
        public ElementChangedMessage(byte x, byte y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public byte X { get; }
        public byte Y { get; }
        public byte Value { get; }

        public byte GetId()
        {
            return 1;
        }

        public byte[] Serialize()
        {
            return new byte[] { X, Y, Value };
        }
    }
}
