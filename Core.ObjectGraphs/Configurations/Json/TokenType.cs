namespace Core.ObjectGraphs.Configurations.Json
{
   public enum TokenType
   {
      None,
      ObjectOpen,
      ObjectClose,
      ArrayOpen,
      ArrayClose,
      Colon,
      Comma,
      Name,
      String,
      StringSingle,
      Number,
      True,
      False,
      Null
   }
}