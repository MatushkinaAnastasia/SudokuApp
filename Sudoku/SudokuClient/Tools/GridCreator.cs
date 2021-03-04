using SudokuClient.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UtilsLibrary.Data;

namespace SudokuClient.Tools
{
	public static class GridCreator
	{
		public static TextBox[,] CreateGrid(Grid grid, Action<TextBox, KeyEventArgs> handler)
		{
			const int _size = 9;
			var tbs = new TextBox[_size, _size];

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


							int x = i * 3 + a;
							int y = j * 3 + b;

							var text_box = new TextBox
							{
								TextAlignment = TextAlignment.Center,
								HorizontalAlignment = HorizontalAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center,
								FontSize = 24,
								BorderThickness = new Thickness(0),
								MaxLength = 1,
								Width = 40,
								Tag = new TextBoxParametres(x, y),	
							};
							text_box.PreviewKeyDown += (s, e) => handler?.Invoke((TextBox)s, e);
							tbs[x, y] = text_box;

							little_grid.Children.Add(little_border);
							Grid.SetRow(little_border, a);
							Grid.SetColumn(little_border, b);

							little_border.Child = text_box;
						}
					}
				}
			}

			return tbs;
		}
	}
}
