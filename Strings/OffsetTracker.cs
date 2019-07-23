namespace Core.Strings
{
   public class OffsetTracker
   {
      int offset;

      public OffsetTracker()
      {
         offset = 0;
      }

      public int Evaluate(int oldLength, int newLength)
      {
         offset += newLength - oldLength;
         return offset;
      }
   }
}