using System.Windows.Forms;

namespace Core.WinForms.Documents
{
   public class DocumentConfiguration
   {
      public DocumentConfiguration(RichTextBox textBox, string extension, string documentName, string fontName = "Consolas",
         float fontSize = 12f, bool displayFileName = true, string filter = "")
      {
         TextBox = textBox;
         Extension = extension;
         DocumentName = documentName;
         FontName = fontName;
         FontSize = fontSize;
         DisplayFileName = displayFileName;
         Filter = filter;
      }

      public RichTextBox TextBox { get; }

      public string Extension { get; }

      public string DocumentName { get; }

      public string FontName { get; }

      public float FontSize { get; }

      public bool DisplayFileName { get; }

      public string Filter { get; }

      public Document Document(Form form) => new(form, TextBox, Extension, DocumentName, FontName, FontSize, DisplayFileName, Filter);
   }
}