using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using UtilsLibrary.Data;

namespace GameServer
{
	public class DatabaseWorker
	{
		private const byte _size = 9;
		private readonly string _connectionString;

		public DatabaseWorker(string path)
		{
			_connectionString = new SQLiteConnectionStringBuilder
			{
				DataSource = path,
			}.ConnectionString;
		}

		public SudokuCell[,] GetData()
		{
			using var conn = new SQLiteConnection(_connectionString);
			var sCommand = new SQLiteCommand()
			{
				Connection = conn,
				CommandText = @"SELECT numbers FROM sudoku_data ORDER BY RANDOM() LIMIT 1;"
			};

			conn.Open();

			var numbers = (string)sCommand.ExecuteScalar();

			string[] nine_lines = numbers.Split('!');

			var data = new SudokuCell[_size, _size];
			for (byte i = 0; i < _size; i++)
			{
				var t = nine_lines[i];
				for (byte j = 0; j < _size; j++)
				{
					var symbol = t[j];
					SudokuCell cell;

					if (symbol != '*')
					{
						cell = new SudokuCell(i, j, byte.Parse(symbol.ToString()), true);
					}
					else
					{
						cell = new SudokuCell(i, j, 0, false);
					}

					data[i, j] = cell;
				}

			}
			return data;
		}
	}
}
