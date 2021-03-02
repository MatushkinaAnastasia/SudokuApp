using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuApp.Views
{
	public partial class SudokuField : Window
	{
		private readonly int _size;
		private int[,] _data;
		private readonly string _connectionString;
		public SudokuField()
		{
			InitializeComponent();

			_connectionString = new SQLiteConnectionStringBuilder
			{
				DataSource = @"C:\Users\nasya\source\repos\Sudoku\SudokuApp\data.sqlite",
			}.ConnectionString;

			_size = 9;
			_data = GetData();
			CreateGrid();
		}

		private int[,] GetData()
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

			var data = new int[_size, _size];
			for (int i = 0; i < _size; i++)
			{
				var t = nine_lines[i];
				for (int j = 0; j < _size; j++)
				{
					var symbol = t[j];
					if (symbol != '*')
					{
						data[i, j] = int.Parse(symbol.ToString());
					}
				}

			}
			return data;
		}

		private void CreateGrid()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					var border = new Border
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(1),
					};
					Grid.SetRow(border, i);
					Grid.SetColumn(border, j);
					grid.Children.Add(border);

					Grid little_grid = new Grid();
					border.Child = little_grid;

					for (int k = 0; k < 3; k++)
					{
						little_grid.RowDefinitions.Add(new RowDefinition());
						little_grid.ColumnDefinitions.Add(new ColumnDefinition());
					}

					for (int a = 0; a < 3; a++)
					{
						for (int b = 0; b < 3; b++)
						{
							var little_border = new Border
							{
								BorderBrush = Brushes.Black,
								BorderThickness = new Thickness(0.6),
							};

							var value = _data[i * 3 + a, j * 3 + b].ToString();

							var text_box = new TextBox
							{
								TextAlignment = TextAlignment.Center,
								HorizontalAlignment = HorizontalAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center,
								FontSize = 24,
								BorderThickness = new Thickness(0),
								MaxLength = 1,
								Width = 40,
							};

							if (value == "0")
							{
								text_box.Text = "";
							}
							else
							{
								text_box.Text = value;
								text_box.IsEnabled = false;
								text_box.Foreground = Brushes.Black;
								text_box.FontWeight = FontWeights.UltraBold;
							}

							little_grid.Children.Add(little_border);
							Grid.SetRow(little_border, a);
							Grid.SetColumn(little_border, b);

							little_border.Child = text_box;
						}
					}
				}
			}
		}

		private void BackToMenu(object sender, RoutedEventArgs e)
		{
			var menu = new Menu();
			menu.Show();
			Close();
		}

		private void RandomField(object sender, RoutedEventArgs e)
		{
			grid.Children.Clear();
			UpdateLayout();
			_data = GetData();
			CreateGrid();
		}
	}
}

