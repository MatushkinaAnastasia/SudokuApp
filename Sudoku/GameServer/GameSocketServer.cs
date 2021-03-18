using GameServer.Events;
using GameServer.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UtilsLibrary.Data;

namespace GameServer
{
    public class GameSocketServer
    {
        private readonly SudokuCell[,] _data;
        private readonly string _roomName;
        private readonly List<Client> _clients;
        private readonly object _locker;
        private Socket _listener;
        private Thread _thread;


        public GameSocketServer(IPEndPoint address, SudokuCell[,] data, string roomName)
        {
            Address = address;
            _data = data;
            _roomName = roomName;
            _clients = new List<Client>();
            _locker = new object();
        }

        public IPEndPoint Address { get; }


        public void Start()
        {
            _listener = new Socket(Address.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = 1024
            };
            _listener.Bind(Address);
            _listener.Listen(10);
            _thread = new Thread(Run);
            _thread.Start();
            Console.WriteLine($"{Address.Address}:{Address.Port}");
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    var socket = _listener.Accept();
                    var client = new Client(socket);

                    client.ElementChanging += ElementChanged;
                    client.ConnectionClosing += ConnectionClosed;
                    client.ConnectionRefusal += ConnectionRefused;

                    try
                    {
                        client.SendMessage(new InitializationMessage(_data, _roomName));
                    }
                    catch
                    {
                        socket.Dispose();
                        continue;
                    }

                    lock (_locker)
                    {
                        _clients.Add(client);
                    }
                }
            }
            catch { /*should close*/ }
        }

        public void Stop()
        {
            foreach (var client in _clients)
            {
                try
                {
                    client.Close();
                }
                catch { /*ignored*/ }
            }

            _listener.Close();
            _listener.Dispose();
        }


        private void ElementChanged(object sender, ElementChangingEventArgs e)
        {
            var x = e.X;
            var y = e.Y;
            var value = e.Value;

            if (x > 8 ||y > 8 || value > 9 || _data[x, y].IsPermanent)
            {
                return;
            }

            _data[x, y].Value = value;

            IMessage message = new ElementChangedMessage(x, y, value);
            BroadcastMessage(message);

            if (SudokuFieldIsFull())
            {
                if (Check())
                {
                    IMessage messageWin = new WinMessage();
                    BroadcastMessage(messageWin);
                }
            }
        }

        private void BroadcastMessage(IMessage message)
        {
            lock (_locker)
            {
                foreach (var client in _clients)
                {
                    try
                    {
                        client.SendMessage(message);
                    }
                    catch { /*ignored*/ }
                }
            }
        }

        private void ConnectionRefused(object sender, ConnectionRefusalEventArgs e)
        {
            lock (_locker)
            {
                _clients.Remove(e.Client);
            }
        }

        private void ConnectionClosed(object sender, ConnectionClosingEventArgs e)
        {
            lock (_locker)
            {
                _clients.Remove(e.Client);
            }
        }

        // fast check
        private bool SudokuFieldIsFull()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_data[i, j].Value == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool Check()
        {
            var arr = new byte[9];

            //проверка строк
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    arr[j] = _data[i, j].Value;
                }
                if (IsValid(arr) == false)
                {
                    return false;
                }
            }

            //проверка столбцов
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    arr[j] = _data[j, i].Value;
                }
                if (IsValid(arr) == false)
                {
                    return false;
                }
            }

            //проверка чисел в трешках
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var k = 0;
                    for (int a = 0; a < 3; a++)
                    {
                        for (int b = 0; b < 3; b++)
                        {
                            int x = i * 3 + a;
                            int y = j * 3 + b;
                            arr[k] = _data[x, y].Value;
                            k++;
                        }
                    }
                    if (IsValid(arr) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //проверяет повторяются ли числа в массиве
        private bool IsValid(byte[] arr)
        {
            //Console.WriteLine($"Полученный массив: {arr[0]} {arr[1]} {arr[2]} {arr[3]} {arr[4]} {arr[5]} {arr[6]} {arr[7]}");
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[i] == arr[j])
                    {
                        //Console.WriteLine($"Элементы {arr[i]} и {arr[j]} не уникальны!");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}