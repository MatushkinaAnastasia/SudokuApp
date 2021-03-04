using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UtilsLibrary.Servers
{
	public class SocketServer : IServer
	{
		private readonly Socket _tcpSocket;
		private readonly List<Task> _tasks;

		public SocketServer(IPAddress ip, int port)
		{
			_tcpSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_tcpSocket.Bind(new IPEndPoint(ip, port));
			_tasks = new List<Task>();
		}

		public void Run(IMessageHandler handler, CancellationToken cancellationToken)
		{
			if (handler is null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			_tcpSocket.Listen(10);

			var cancelWaitTask = Task.Run(() =>
			{
				using var resetEvent = new ManualResetEvent(false);
				cancellationToken.Register(() => resetEvent.Set());
				resetEvent.WaitOne();
			});

			while (!cancellationToken.IsCancellationRequested)
			{
				var socketAcceptTask = _tcpSocket.AcceptAsync();
				Task.WaitAny(socketAcceptTask, cancelWaitTask);

				if (cancelWaitTask.IsCompleted)
				{
					break;
				}

				var task = Task.Run(() => ReceiveMessage(handler, socketAcceptTask.Result));

				_tasks.Add(task);
			}

			Task.WaitAll(_tasks.ToArray());
		}

		//получение сообщений
		private void ReceiveMessage(IMessageHandler handler, Socket socket)
		{
			try
			{
				var buffer = new byte[1024];
				int size = socket.Receive(buffer);
				var data = new byte[size];
				Array.Copy(buffer, 0, data, 0, size);

				handler.Handle(data, socket);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Recieving message socket: " + ex.Message);
			}
			finally
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
				socket.Dispose();
			}
		}
	}
}
