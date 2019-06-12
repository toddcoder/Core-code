namespace Core.Objects
{
   public class StandardFormatter : IFormatter
   {
      string format;

      public StandardFormatter(string format) => this.format = $"{{0{format}}}";

      public string Format(object obj) => string.Format(format, obj);
   }
}