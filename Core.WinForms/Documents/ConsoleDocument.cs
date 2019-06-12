using System;
using System.IO;
using System.Windows.Forms;
using Core.WinForms.Consoles;

namespace Core.WinForms.Documents
{
   public class ConsoleDocument : IDisposable
   {
      readonly RichTextBox editTextBox;
      readonly RichTextBox consoleTextBox;
      readonly TextBoxConsole console;
      readonly Document document;
      readonly TextWriter writer;
      readonly TextReader reader;

      public ConsoleDocument(Form form, DocumentConfiguration documentConfiguration, ConsoleConfiguration consoleConfiguration)
      {
         editTextBox = documentConfiguration.TextBox;
         consoleTextBox = consoleConfiguration.TextBox;
         console = consoleConfiguration.Console(form);
         document = documentConfiguration.Document(form);

         writer = console.Writer();
         reader = console.Reader();

         document.StandardMenus();
      }

      public Menus Menus => document.Menus;

      public RichTextBox EditTextBox => editTextBox;

      public RichTextBox ConsoleTextBox => consoleTextBox;

      public TextBoxConsole Console => console;

      public Document Document => document;

      public TextWriter Writer => writer;

      public TextReader Reader => reader;

      public void Begin(Form form)
      {
         document.Menus.CreateMainMenu(form);
         document.Menus.StandardContextEdit(document);
      }

      public void Dispose()
      {
         writer?.Dispose();
         reader?.Dispose();
      }
   }
}