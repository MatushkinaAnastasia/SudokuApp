using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuClient.Tools
{
	public class TextBoxParametres
	{
		public TextBoxParametres(byte x, byte y)
		{
			X = x;
			Y = y;
		}

		public byte X { get; private set; }
		public byte Y { get; private set; }
	}
}
