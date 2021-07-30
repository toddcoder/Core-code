namespace Core.Markup.Rtf
{
   public class ColorDescriptor
   {
      protected int descriptor;

      public ColorDescriptor(int descriptor)
      {
         this.descriptor = descriptor;
      }

      public int Value => descriptor;
   }
}