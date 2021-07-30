namespace Core.Markup.Rtf
{
   public class FontDescriptor
   {
      protected int descriptor;

      public FontDescriptor(int descriptor)
      {
         this.descriptor = descriptor;
      }

      public int Value => descriptor;
   }
}