namespace Core.Strings
{
   public static class StringStreamFunctions
   {
      public static StringStream stream() => new StringStream();

      public static string end() => "\r\n";
   }
}