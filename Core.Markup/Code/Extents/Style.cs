namespace Core.Markup.Code.Extents
{
   public class Style : Extent
   {
      public Style(string name)
      {
         Name = name;
      }

      public string Name { get; }
   }
}