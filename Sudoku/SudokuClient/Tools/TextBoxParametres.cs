using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuClient.Tools
{
	public class TextBoxParametres
	{
		public TextBoxParametres(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; private set; }
		public int Y { get; private set; }


	}
}
