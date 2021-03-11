using System;
using System.Net;
using System.Net.Sockets;

namespace UtilsLibrary.Servers
{
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

        public byte[] SendAndRecieve(byte[] data)
        {
            Socket socket = null;
            try
            {
                socket = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(_remoteEP);

                socket.Send(data);

                var buffer = new byte[2048];
                int size = socket.Receive(buffer);
                var recievedData = new byte[size];
                Array.Copy(buffer, 0, recievedData, 0, size);
                return recievedData;
                
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                //Console.WriteLine("SocketException : {0}", se.ToString());
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            finally
            {
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
                socket?.Dispose();
            }
            return null;
        }
    }
}
