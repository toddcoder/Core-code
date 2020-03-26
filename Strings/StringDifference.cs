namespace Core.Strings
{
   public class StringDifference
   {
      public static StringDifference Empty => new StringDifference(false, false);

      public StringDifference(bool caseDiffers, bool identical)
      {
         CaseDiffers = caseDiffers;
         Identical = identical;

         Left = string.Empty;
         Right = string.Empty;
      }

      public StringDifference(string left, string right)
      {
         Left = left;
         Right = right;

         CaseDiffers = left.CaseDiffers(right);
         Identical = left == right;
      }

      public bool CaseDiffers { get; }

      public bool Identical { get; }

      public string Left { get; }

      public string Right { get; }

      public void Deconstruct(out bool caseDiffers, out bool identical)
      {
         caseDiffers = CaseDiffers;
         identical = Identical;
      }
   }
}