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
      Number,
      True,
      False,
      Null
   }
}