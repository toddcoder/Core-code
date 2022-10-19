namespace Core.Markup.Rtf;

public class ColorDescriptor : Descriptor
{
   public ColorDescriptor(int descriptor) : base(descriptor)
   {
   }

   public ForegroundColorDescriptor Foreground => new(descriptor);

   public BackgroundColorDescriptor Background => new(descriptor);
}