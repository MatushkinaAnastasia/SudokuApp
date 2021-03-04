namespace UtilsLibrary.Data
{
	public class SudokuCell
	{
		public SudokuCell(byte x, byte y, byte value, bool isPermanent)
		{
			X = x;
			Y = y;
			Value = value;
			IsPermanent = isPermanent;
		}

		public SudokuCell(byte[] array)
		{
			X = array[0];
			Y = array[1];
			Value = array[2];
			IsPermanent = array[3] == 1;
		}

		public byte X { get; private set; }
		public byte Y { get; private set; }

		/// <summary>
		/// 0 if empty.
		/// 1 - 9 is normal.
		/// </summary>
		public byte Value { get; set; }
		public bool IsPermanent { get; private set; }

		public byte[] ToByteArray()
		{
			byte isP = IsPermanent ? (byte)1 : (byte)0;
			return new byte[] { X, Y, Value, isP };
		}
	}
}
