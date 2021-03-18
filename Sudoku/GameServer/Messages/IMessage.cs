using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Messages
{
    public interface IMessage
    {
        byte GetId();
        byte[] Serialize();
    }
}
