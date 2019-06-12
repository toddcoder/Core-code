namespace Core.RegularExpressions.Parsers
{
   public static class ParserHelpers
   {
      public static string Enclose(this string source, bool enclose) => enclose ? $"({source})" : source;
   }
}