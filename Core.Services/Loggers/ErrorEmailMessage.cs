using Core.Markup.Xml;
using Core.Strings;

namespace Core.Services.Loggers;

public class ErrorEmailMessage
{
   public const string STYLE_BODY = "background-color: wheat;";
   public const string STYLE_H3 = "text-align: center; font: bold tahoma";
   public const string STYLE_HEADER = "background-color: salmon; font: bold tahoma; padding: 4px; " +
      "border: solid 4px white; margin: 0 0 0 0;";
   public const string STYLE_MESSAGE = "font-family: Consolas, Courier New, sans-serif; margin-top: 0; " +
      "margin-bottom: 10px; padding: 4px";

   protected MarkupBuilder builder;
   protected Element table;
   protected string fontSize;

   public ErrorEmailMessage(string title, string fontSize = "")
   {
      builder = new MarkupBuilder("html") { Tidy = false, IncludeHeader = false, IsHtml = true };
      this.fontSize = fontSize;

      var root = builder.Root;
      var body = root.Children.Add("body");
      body.Attributes.Add("style", STYLE_BODY);
      var h3 = body.Children.Add("h3", title);
      h3.Attributes.Add("style", getStyle(STYLE_H3));
      table = body.Children.Add("table");
      table.Attributes.Add("width", "100%");
   }

   protected string getStyle(string style) => fontSize.IsNotEmpty() ? $"{style}; font-size: {fontSize}" : style;

   public void Add(string header, string message)
   {
      var headerElement = new Element { Name = "span", Text = $"~{header}~" };
      headerElement.Attributes.Add("style", getStyle(STYLE_HEADER));
      var messageElement = new Element { Name = "p", Text = message };
      messageElement.Attributes.Add("style", getStyle(STYLE_MESSAGE));
      table.Children.Add("tr").Children.Add("td").Children.Add(headerElement);
      table.Children.Add("tr").Children.Add("td").Children.Add(messageElement);
   }

   public override string ToString() => builder.ToString();
}