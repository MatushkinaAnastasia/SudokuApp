using System;
using System.Collections.Generic;
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
		public SudokuField()
		{
			InitializeComponent();
			CreateGrid();
		}

		private void CreateGrid()
		{
			var r = new Random();

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


							var text_box = new TextBox
							{
								Text = r.Next(1, 10).ToString(),
								TextAlignment = TextAlignment.Center,
								HorizontalAlignment = HorizontalAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center,
								FontSize = 24,
								BorderThickness = new Thickness(0),
								MaxLength = 1,
							};

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
	}
}

