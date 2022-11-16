namespace Core.Matching.Parsers;

public static class ParserExtensions
{
   public static string Enclose(this string source, bool enclose) => enclose ? $"({source})" : source;
}