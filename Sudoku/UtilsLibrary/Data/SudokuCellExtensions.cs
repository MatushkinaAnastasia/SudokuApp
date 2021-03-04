using System;

namespace UtilsLibrary.Data
{
	public static class SudokuCellExtensions
	{
		public static byte[] ConvertToByteArray(this SudokuCell[,] cells)
		{
			int size = cells.GetLength(0);
			var sizeOfOneSudokuCell = new SudokuCell(1, 1, 1, true).ToByteArray().Length;
			var result = new byte[sizeOfOneSudokuCell * size * size];
			int resultIndex = 0;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					var oneCell = cells[i, j].ToByteArray();
					Array.Copy(oneCell, 0, result, resultIndex, oneCell.Length);
					resultIndex += oneCell.Length;
				}
			}

			return result;
		}

		public static SudokuCell[,] ConvertToSudokuCellArray(this byte[] array)
		{
			var length = array.Length;
			var sizeOfOneSudokuCell = new SudokuCell(1, 1, 1, true).ToByteArray().Length;
			int count = length / sizeOfOneSudokuCell; // = 81
			int size = 9; // sqrt of count.
			var result = new SudokuCell[size, size];

			int arrayIndex = 0;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					var oneSudokuCellBytes = new byte[sizeOfOneSudokuCell];
					Array.Copy(array, arrayIndex, oneSudokuCellBytes, 0, sizeOfOneSudokuCell);
					arrayIndex += sizeOfOneSudokuCell;

					var sudoku = new SudokuCell(oneSudokuCellBytes);
					result[i, j] = sudoku;
				}
			}

			return result;
		}
	}
}
