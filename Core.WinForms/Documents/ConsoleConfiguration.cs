using System.Windows.Forms;
using Core.WinForms.Consoles;

namespace Core.WinForms.Documents
{
   public class ConsoleConfiguration
   {
      public ConsoleConfiguration(RichTextBox textBox, string fontName = "Consolas", float fontSize = 10f,
         TextBoxConsole.ConsoleColorType consoleColorType = TextBoxConsole.ConsoleColorType.Windows)
      {
         TextBox = textBox;
         FontName = fontName;
         FontSize = fontSize;
         ConsoleColorType = consoleColorType;
      }

      public RichTextBox TextBox { get; }

      public string FontName { get; }

      public float FontSize { get; }

      public TextBoxConsole.ConsoleColorType ConsoleColorType { get; }

      public TextBoxConsole Console(Form form) => new(form, TextBox, FontName, FontSize, ConsoleColorType);
   }
}