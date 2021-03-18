using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using UtilsLibrary;
using UtilsLibrary.Data;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ip = NetworkUtils.GetMyIp();
            var port = NetworkUtils.GetFreePort();

            var address = new IPEndPoint(ip, port);

            string roomName = args.Length > 0 && args[0].Length > 0 && args[0].Length <= 30 ? args[0] : $"{address.ToString()}";

            SudokuCell[,] data = null;
            if (args.Length > 1)
            {
                var path = args[1];
                try
                {
                    data = LoadDataFromFile(path);
                }
                catch { data = null; } // user friendly to say it to user but we are too lazy
            }

            if (data is null)
            {
                data = LoadRandomData();
            }

            var server = new GameSocketServer(address, data, roomName);
            server.Start();
            Console.Read();
            server.Stop();
        }

        private static SudokuCell[,] LoadRandomData()
        {
            var pathToDB = ConfigurationManager.AppSettings.Get("pathToDB");
            var db = new DatabaseWorker(pathToDB);
            var data = db.GetData();
            
            return data;
        }

        private static SudokuCell[,] LoadDataFromFile(string path)
        {
            using var streamReader = new StreamReader(path);
            var data = streamReader.ReadToEnd();
            var byteData = Encoding.UTF8.GetBytes(data);

            try
            {
                return SudokuCellExtensions.ConvertToSudokuCellArray(byteData);
            }
            catch
            {
                return null;
            }
        }
    }
}
