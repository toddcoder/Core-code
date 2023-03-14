namespace Core.Objects;

public class NewFormatter : IFormatter
{
   protected string format;

   public NewFormatter(string format) => this.format = format;

   public string Format(object obj) => obj.FormatAs(format);
}