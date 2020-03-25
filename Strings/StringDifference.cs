namespace Core.Strings
{
   public class StringDifference
   {
      public static StringDifference Empty => new StringDifference(false, false, false);

      public StringDifference(bool same, bool caseDiffers, bool identical)
      {
         Same = same;
         CaseDiffers = caseDiffers;
         Identical = identical;

         Left = string.Empty;
         Right = string.Empty;
      }

      public StringDifference(string left, string right)
      {
         Left = left;
         Right = right;

         Same = left.Same(right);
         CaseDiffers = left.CaseDiffers(right);
         Identical = left == right;
      }

      public bool Same { get; }

      public bool CaseDiffers { get; }

      public bool Identical { get; }

      public string Left { get; }

      public string Right { get; }

      public void Deconstruct(out bool same, out bool caseDiffers, out bool identical)
      {
         same = Same;
         caseDiffers = CaseDiffers;
         identical = Identical;
      }
   }
}