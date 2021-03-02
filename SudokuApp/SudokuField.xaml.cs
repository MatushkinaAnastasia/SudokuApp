using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuApp
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class SudokuField : Window
	{
		private readonly int _size;
		private readonly int[,] _data;
		public SudokuField()
		{
			InitializeComponent();

			_size = 9;
			_data = GetDataFromFile(@"C:\Users\nasya\source\repos\SudokuApp\data.txt");
			for (int i = 0; i < _size; i++)
			{
				for (int j = 0; j < _size; j++)
				{
					Console.Write(_data[i, j]);
				}
				Console.WriteLine();
			}
			CreateGrid();
		}

		private void CreateGrid()
		{
			for (int i = 0; i < 3; i++)
			{
				grid.RowDefinitions.Add(new RowDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());
			}

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					var border = new Border
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(1),
					};
					grid.Children.Add(border);
					Grid.SetRow(border, i);
					Grid.SetColumn(border, j);

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

							little_grid.Children.Add(text_box);
							Grid.SetRow(text_box, a);
							Grid.SetColumn(text_box, b);
						}
					}

				}
			}

		}

		private int[,] GetDataFromFile(string path)
		{
			var array = new int[_size, _size];
			using (var reader = new StreamReader(path))
			{
				for (int i = 0; i < _size; i++)
				{
					var line = reader.ReadLine();
					for (int j = 0; j < _size; j++)
					{
						var symbol = line[j];
						if (symbol != '*')
						{
							array[i, j] = int.Parse(symbol.ToString());
						}
					}
				}
			}

			return array;
		}
	}
}

