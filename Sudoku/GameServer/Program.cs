using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
	class Program : IMessageHandler
	{
        private readonly CancellationTokenSource _cancellationTokenSource;
        // _data
        private readonly IPAddress _ip;
        private readonly int _port;
        public Program(string ip, string port)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _ip = IPAddress.Parse(ip);
            _port = int.Parse(port);
        }

        private static void Main(string[] args)
        {
            var program = new Program(args[0], args[1]);
            Console.CancelKeyPress += (sender, e) =>
            {
                try
                {
                    program.Cancel();
                }
                finally
                {
                    e.Cancel = true;
                }
            };
            program.Run();
        }

        public void Run()
        {
            IServer server = new GameServer(_ip, _port);
            var task = Task.Run(() => server.Run(this, _cancellationTokenSource.Token));

            Console.WriteLine("Сервер находится по адресу:" + _ip + ":" + _port);
            Console.WriteLine("-Сервер-в-режиме-ожидания-");

            Task.WaitAll(task);
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        //работа с полученными данными
		public void Handle(byte[] message)
		{
            var command = message[0];

            if (command == 1)
			{
                var x = message[1];
                var y = message[2];
                var value = message[3];
				Console.WriteLine("data recieved:");
				Console.WriteLine($"x = {x}");
				Console.WriteLine($"y = {y}");
				Console.WriteLine($"value = {value}");
                //_data[x,y] =value;
                //сказать всем
			}
		}
	}
}
