using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace SudokuClient.GameUtils
{
    public sealed class GameServerWrapper
    {
        public readonly string _gameServerPath;
        private readonly string _nameOfRoom;
        private readonly string _path;
        private Process _process;

        public GameServerWrapper(string nameOfRoom, string path)
        {
            _gameServerPath = ConfigurationManager.AppSettings.Get("pathToGameServer");
            _nameOfRoom = nameOfRoom;
            _path = path;
        }

        public IPEndPoint IPEndPoint { get; private set; }

        public bool Running { get => _process != null; }

        public void Start()
        {
            if (!Running)
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = _gameServerPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };
                info.ArgumentList.Add(_nameOfRoom);
                info.ArgumentList.Add(_path);

                _process = Process.Start(info);
                IPEndPoint = ReadAddress();
            }
            else
            {
                throw new InvalidOperationException("server is already started");
            }
        }

        private IPEndPoint ReadAddress()
        {
            StreamReader reader = _process.StandardOutput;
            string address = reader.ReadLine();
            return IPEndPoint.Parse(address);
        }

        public void Stop()
        {
            if (Running)
            {
                StreamWriter writer = _process.StandardInput;
                writer.Write('\n');
                _process.WaitForExit();
                _process = null;
            }
        }
    }
}
