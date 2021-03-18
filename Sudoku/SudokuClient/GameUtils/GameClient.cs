using SudokuClient.GameUtils.Events;
using SudokuClient.GameUtils.Messages;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UtilsLibrary;

namespace SudokuClient.GameUtils
{
    public class GameClient
    {
        /* List of IDs of incoming messages. */
        private const int InitializedMessageId = 0x00;
        private const int ElementChangedMessageId = 0x01;
        private const int WinnedMessageId = 0x02;
        /* List of lengths of incoming messages. */
        private const int InitializedMessageLength = 324;
        private const int ElementChangedMessageLength = 3;
        //private const int WinnedMessageLength = 0;

        private TcpClient _tcpClient;
        private Thread _thread;
        private bool _manualRefusal;

        public GameClient() { }

        public delegate void ConnectionFailedEventHandler(object sender, ConnectionFailedEventArgs e);
        public delegate void ConnectionSucceededEventHandler(object sender, ConnectionSucceededEventArgs e);
        public delegate void InitializeEventHandler(object sender, InitializeEventArgs e);
        public delegate void ElementChangedEventHandler(object sender, ElementChangedEventArgs e);
        public delegate void WinnedEventHandler(object sender, WinnedEventArgs e);
        public delegate void ConnectionClosedEventHandler(object sender, ConnectionClosedEventArgs e);
        public delegate void DisconnectedEventHandler(object sender, DisconnectedEventArgs e);
        public delegate void ConnectionRefusedEventHandler(object sender, ConnectionRefusedEventArgs e);

        public event ConnectionFailedEventHandler ConnectionFailed;
        public event ConnectionSucceededEventHandler ConnectionSucceeded;
        public event InitializeEventHandler Initialized;
        public event ElementChangedEventHandler ElementChanged;
        public event WinnedEventHandler Winned;
        public event ConnectionClosedEventHandler ConnectionClosed;
        public event DisconnectedEventHandler Disconnected;
        public event ConnectionRefusedEventHandler ConnectionRefused;

        public void Connect(IPEndPoint ipEndPoint)
        {
            _tcpClient = new TcpClient
            {
                ReceiveBufferSize = 1024
            };
            try
            {
                _tcpClient.Connect(ipEndPoint);
                ConnectionSucceeded?.Invoke(this, new ConnectionSucceededEventArgs());
            }
            catch (Exception e)
            {
                ConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs(e));
                _tcpClient = null;
            }

            if (_tcpClient != null)
            {
                _manualRefusal = false;
                _thread = new Thread(Run);
                _thread.Start();
            }
        }
        private void Run()
        {
            try
            {
                while (true)
                {
                    NetworkStream stream = _tcpClient.GetStream();
                    int messageId = stream.ReadByte();
                    Console.WriteLine(messageId);
                    if (messageId == -1)
                    {
                        ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs());
                        break;
                    }
                    ProcessMessageRecieved(messageId, stream);
                }
            }
            catch (IOException e)
            {
                if (_manualRefusal)
                {
                    Disconnected?.Invoke(this, new DisconnectedEventArgs());
                }
                else
                {
                    ConnectionRefused?.Invoke(this, new ConnectionRefusedEventArgs(e));
                }
            }
            catch
            {
                ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs());
            }
        }

        private void ProcessMessageRecieved(int messageId, NetworkStream stream)
        {
            switch (messageId)
            {
                case InitializedMessageId:
                    ProcessInitializedMessage(stream);
                    break;
                case ElementChangedMessageId:
                    ProcessElementChangedMessage(stream);
                    break;
                case WinnedMessageId:
                    ProcessWinnedMessage();
                    break;
                default:
                    break;
            }
        }


        private void ProcessInitializedMessage(NetworkStream stream)
        {
            byte[] bytes = stream.ReadNBytes(InitializedMessageLength);
            int nameLength = BitConverter.ToInt32(stream.ReadNBytes(4));
            byte[] nameBytes = stream.ReadNBytes(nameLength);
            Initialized?.Invoke(this, new InitializeEventArgs(bytes, nameBytes));
        }

        private void ProcessElementChangedMessage(NetworkStream stream)
        {
            byte[] bytes = stream.ReadNBytes(ElementChangedMessageLength);
            ElementChanged?.Invoke(this, new ElementChangedEventArgs(x: bytes[0], y: bytes[1], value: bytes[2]));
        }

        private void ProcessWinnedMessage()
        {
            Winned?.Invoke(this, new WinnedEventArgs());
        }

        public void SendMessage(IMessage message)
        {
            if (_tcpClient != null)
            {
                try
                {
                    byte[] bytes = message.Serialize();
                    NetworkStream stream = _tcpClient.GetStream();
                    stream.WriteByte(message.Id);
                    stream.Write(bytes, 0, bytes.Length);
                }
                catch (Exception) { /* ingored */ }
            }
        }

        public void Disconnect()
        {
            if (_tcpClient != null)
            {
                _manualRefusal = true;
                _tcpClient.Close();
                _tcpClient.Dispose();
                _tcpClient = null;
                _thread = null;
            }
        }
    }
}
