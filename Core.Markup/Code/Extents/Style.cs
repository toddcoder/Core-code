namespace Core.Markup.Code.Extents
{
   public sealed class Style : Extent
   {
      public Style(string name)
      {
         Name = name;
      }

      public string Name { get; }
   }
}