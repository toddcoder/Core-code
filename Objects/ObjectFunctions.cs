namespace Core.Objects
{
	public static class ObjectFunctions
	{
		public static void swap<T>(ref T left, ref T right)
		{
			var temp = left;
			left = right;
			right = temp;
		}
	}
}