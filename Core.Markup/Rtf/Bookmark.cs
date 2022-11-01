namespace Core.Markup.Rtf;

public class Bookmark
{
   internal Bookmark(string name)
   {
      Name = name;
   }

   public string Name { get; }
}