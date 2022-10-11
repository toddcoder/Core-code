namespace Core.Markup.Rtf;

public class Descriptor
{
   protected int descriptor;

   public Descriptor(int descriptor)
   {
      this.descriptor = descriptor;
   }

   public int Value => descriptor;
}