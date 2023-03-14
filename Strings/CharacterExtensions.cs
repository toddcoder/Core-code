using System.Text;

namespace Core.Strings;

public static class CharacterExtensions
{
   public static string Repeat(this char source, int count)
   {
      var result = new StringBuilder();
      for (var i = 0; i < count; i++)
      {
         result.Append(source);
      }

      return result.ToString();
   }
}