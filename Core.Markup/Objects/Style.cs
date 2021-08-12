namespace Core.Markup.Objects
{
   public sealed class Style : Element
   {
      public Style(string name)
      {
         Name = name;
      }

      public string Name { get; }
   }
}