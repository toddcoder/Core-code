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
      String,
      Number,
      True,
      False,
      Null
   }
}