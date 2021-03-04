using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsLibrary
{
	public static class ArrayWorker
	{
		public static T[] DoubleArrayToLinear<T>(this T[,] source)
		{
			var size = source.GetLength(0);
			var result = new T[size * size];
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					result[i * size + j] = source[i, j];
				}
			}

			return result;
		}

		public static T[,] LinearArrayToDouble<T>(this T[] source, int size)
		{
			var result = new T[size, size];
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					result[i, j] = source[i * size + j];
				}
			}

			return result;
		}
	}
}
