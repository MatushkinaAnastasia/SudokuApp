using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SudokuClient.Tools
{
    public class Communication
    {
        public static void SendData()
        {
            var hostName = Dns.GetHostName();
            var ip = Dns.GetHostEntry(hostName).AddressList[1];
            var port = UtilsLibrary.NetworkUtils.GetFreePort();
            var client = new SocketClient(ip, port);

            var path = UtilsLibrary.GetPath.MakePath("pathToGameServer");

            Process.Start(path, $"{ip} {port}");

            var bytes = new byte[]
            {
                1,
                5,
                7,
                3
            };

            client.Send(bytes);
        }
        public class SocketClient
        {
            private readonly IPEndPoint _remoteEP;
            private readonly IPAddress _ip;
            public SocketClient(IPAddress ip, int port)
            {
                _ip = ip ?? throw new ArgumentNullException(nameof(ip));
                _remoteEP = new IPEndPoint(ip, port);
            }

            public void Send(byte[] data)
            {
                try
                {
                    Socket socket = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(_remoteEP);

                    socket.Send(data);

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket.Dispose();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
        }
    }
}
