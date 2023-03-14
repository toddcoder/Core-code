namespace Core.Objects;

public class StandardFormatter : IFormatter
{
   protected string format;

   public StandardFormatter(string format) => this.format = $"{{0{format}}}";

   public string Format(object obj) => string.Format(format, obj);
}