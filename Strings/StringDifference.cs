namespace Core.Strings
{
   public class StringDifference
   {
      public StringDifference(bool same, bool caseDiffers)
      {
         Same = same;
         CaseDiffers = caseDiffers;
      }

      public bool Same { get; }

      public bool CaseDiffers { get; }
   }
}