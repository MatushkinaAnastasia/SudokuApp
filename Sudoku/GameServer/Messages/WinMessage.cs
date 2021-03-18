 namespace GameServer.Messages
{
    public class WinMessage : IMessage
    {
        public WinMessage()
        {
        }

        public byte GetId()
        {
            return 2;
        }

        public byte[] Serialize()
        {
            return new byte[0];
        }
    }
}
