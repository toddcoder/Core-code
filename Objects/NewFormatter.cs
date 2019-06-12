namespace Core.Objects
{
   public class NewFormatter : IFormatter
   {
      string format;

      public NewFormatter(string format) => this.format = format;

      public string Format(object obj) => obj.FormatAs(format);
   }
}