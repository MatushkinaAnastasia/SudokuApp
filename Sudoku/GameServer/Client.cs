using GameServer.Events;
using GameServer.Messages;
using System.Net.Sockets;
using System.Threading;
using UtilsLibrary;

namespace GameServer
{
    public class Client
    {
        private const int ELEMENT_CHANGE_MESSAGE_ID = 0x00;
        private const int ELEMENT_CHANGE_MESSAGE_SIZE = 3;

        private readonly Socket _socket;
        private readonly Thread _thread;

        public Client(Socket socket)
        {
            _socket = socket;
            _thread = new Thread(Run);
            _thread.Start();
        }

        public delegate void ElementChangingEventHandler(object sender, ElementChangingEventArgs e);
        public delegate void ConnectionClosingEventHandler(object sender, ConnectionClosingEventArgs e);
        public delegate void ConnectionRefusalEventHandler(object sender, ConnectionRefusalEventArgs e);

        public event ElementChangingEventHandler ElementChanging;
        public event ConnectionClosingEventHandler ConnectionClosing;
        public event ConnectionRefusalEventHandler ConnectionRefusal;

        public void SendMessage(IMessage message)
        {
            using var stream = new NetworkStream(_socket);
            stream.WriteByte(message.GetId());
            var bytes = message.Serialize();
            stream.Write(bytes, 0, bytes.Length);
        }

        public void Close()
        {
            _socket.Close();
            _socket.Dispose();
        }

        private void Run(object obj)
        {
            try
            {
                while (true)
                {
                    var stream = new NetworkStream(_socket);
                    int messageId = stream.ReadByte();
                    if (messageId == -1)
                    {
                        ConnectionClosed();
                        break;
                    }
                    ProcessMessage(messageId);
                }
            }
            catch
            {
                ConnectionRefused();
            }
        }


        private void ProcessMessage(int messageId)
        {
            if (messageId == ELEMENT_CHANGE_MESSAGE_ID)
            {
                var stream = new NetworkStream(_socket);
                var bytes = stream.ReadNBytes(ELEMENT_CHANGE_MESSAGE_SIZE);
                var args = ElementChangingEventArgs.Deserialize(this, bytes);
                ElementChanging?.Invoke(this, args);
            }
        }

        private void ConnectionClosed()
        {
            var args = new ConnectionClosingEventArgs(this);
            ConnectionClosing?.Invoke(this, args);
        }

        private void ConnectionRefused()
        {
            var args = new ConnectionRefusalEventArgs(this);
            ConnectionRefusal?.Invoke(this, args);
        }
    }
}