using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Documents
{
   public class Document
   {
      protected const string PATTERN_CRLF = "/r /n | /r | /n; f";

      public static string GetWindowsText(string text) => SetWindowsText(text).ToString("\r\n");

      public static string[] SetWindowsText(string text) => text.Split(PATTERN_CRLF);

      public static Maybe<string> ClipboardText()
      {
         return maybe(Clipboard.ContainsText(TextDataFormat.Text), () => Clipboard.GetText(TextDataFormat.Text));
      }

      protected Form form;
      protected RichTextBox textBox;
      protected string extension;
      protected string documentName;
      protected string formName;
      protected Maybe<FileName> _file;
      protected bool isDirty;
      protected OpenFileDialog openFileDialog;
      protected SaveFileDialog saveFileDialog;
      protected Menus menus;
      protected string fontName;
      protected float fontSize;
      protected bool displayFileName;
      protected Maybe<Colorizer> _colorizer;
      protected string filter;
      protected bool keepClean;

      public event EventHandler OKButtonClicked;
      public event EventHandler CancelButtonClicked;
      public event EventHandler YesButtonClicked;
      public event EventHandler NoButtonClicked;

      public Document(Form form, RichTextBox textBox, string extension, string documentName, string fontName = "Consolas",
         float fontSize = 12f, bool displayFileName = true, string filter = "")
      {
         this.form = form;
         this.textBox = textBox;
         this.extension = extension.Substitute("^ '.'; f", "");
         this.documentName = documentName;
         this.fontName = fontName;
         this.fontSize = fontSize;
         this.displayFileName = displayFileName;
         this.filter = filter.IsEmpty() ? $"{documentName} files (*.{this.extension})|*.{this.extension}|All files (*.*)|*.*" : filter;

         isDirty = false;
         formName = this.form.Text;
         _file = none<FileName>();
         _colorizer = none<Colorizer>();
         keepClean = false;

         initialize();
      }

      public bool KeepClean
      {
         get => keepClean;
         set => keepClean = value;
      }

      protected void initialize()
      {
         textBox.TextChanged += (_, _) =>
         {
            if (!keepClean)
            {
               Dirty();
            }

            if (_colorizer.If(out var c))
            {
               c.Colorize(textBox);
            }
         };

         form.FormClosing += (_, e) => Close(e);

         openFileDialog = new OpenFileDialog
         {
            AddExtension = true,
            CheckFileExists = true,
            CheckPathExists = true,
            DefaultExt = extension,
            Filter = filter,
            RestoreDirectory = true,
            Title = $"Open {documentName} file"
         };
         saveFileDialog = new SaveFileDialog
         {
            AddExtension = true,
            CheckPathExists = true,
            DefaultExt = extension,
            Filter = openFileDialog.Filter,
            RestoreDirectory = true,
            Title = $"Save {documentName} file"
         };

         menus = new Menus();

         initializeTextBox();

         DisplayFileName();
      }

      public Maybe<Colorizer> Colorizer
      {
         get => _colorizer;
         set => _colorizer = value;
      }

      public void StandardFileMenu()
      {
         menus.Menu("&File");
         menus.Menu("File", "New...", (_, _) => New(), "^N");
         standardItems();
      }

      public void StandardFileMenu(EventHandler handler)
      {
         menus.Menu("&File");
         menus.Menu("File", "New", handler, "^N");
         standardItems();
      }

      protected void standardItems()
      {
         menus.Menu("File", "Open...", (_, _) => Open(), "^O");
         menus.Menu("File", "Save", (_, _) => Save(), "^S");
         menus.Menu("File", "Save As...", (_, _) => SaveAs());
         menus.MenuSeparator("File");
         menus.Menu("File", "Exit", (_, _) => form.Close(), "%F4");
      }

      public void StandardEditMenu()
      {
         menus.Menu("&Edit");
         menus.Menu("Edit", "Undo", (_, _) => Undo(), "^Z");
         menus.Menu("Edit", "Redo", (_, _) => Redo());
         menus.MenuSeparator("Edit");
         menus.Menu("Edit", "Cut", (_, _) => Cut(), "^X");
         menus.Menu("Edit", "Copy", (_, _) => Copy(), "^C");
         menus.Menu("Edit", "Paste", (_, _) => Paste(), "^V");
         menus.Menu("Edit", "Delete", (_, _) => Delete());
         menus.MenuSeparator("Edit");
         menus.Menu("Edit", "Select All", (_, _) => SelectAll(), "^A");
      }

      public void StandardMenus()
      {
         StandardFileMenu();
         StandardEditMenu();
      }

      public void StandardMenus(EventHandler fileNewHandler)
      {
         StandardFileMenu(fileNewHandler);
         StandardEditMenu();
      }

      public void StandardContextEdit() => menus.StandardContextEdit(this);

      public Menus Menus => menus;

      public void RenderMainMenu() => menus.CreateMainMenu(form);

      public void RenderContextMenu() => RenderContextMenu(textBox);

      public void RenderContextMenu(Control control) => menus.CreateContextMenu(control);

      protected void initializeTextBox()
      {
         textBox.AcceptsTab = true;
         textBox.DetectUrls = false;
         textBox.Dock = DockStyle.Fill;
         textBox.Font = new Font(fontName, fontSize, FontStyle.Regular);
         textBox.HideSelection = false;
         textBox.ShowSelectionMargin = true;
         textBox.WordWrap = false;
         textBox.ScrollBars = RichTextBoxScrollBars.Both;
      }

      public Maybe<string> FileName => _file.Map(f => f.ToString());

      public bool IsDirty => isDirty;

      public void Dirty()
      {
         isDirty = true;
         DisplayFileName();
      }

      public void Clean()
      {
         isDirty = false;
         DisplayFileName();
      }

      public virtual void New()
      {
         if (isDirty)
         {
            var result = getSaveResponse();
            switch (result)
            {
               case DialogResult.Yes:
                  YesButtonClicked?.Invoke(this, new EventArgs());
                  Save();
                  break;
               case DialogResult.No:
                  NoButtonClicked?.Invoke(this, new EventArgs());
                  textBox.Clear();
                  Clean();
                  break;
               default:
                  return;
            }
         }

         textBox.Clear();
         Clean();
         _file = none<FileName>();
         DisplayFileName();
      }

      public virtual void Open()
      {
         if (openFileDialog.ShowDialog() == DialogResult.OK)
         {
            Open(openFileDialog.FileName);
         }
      }

      public virtual bool OpenIf(out DialogResult dialogResult)
      {
         dialogResult = openFileDialog.ShowDialog();
         if (dialogResult == DialogResult.OK)
         {
            OKButtonClicked?.Invoke(this, new EventArgs());
            Open(openFileDialog.FileName);
            return true;
         }
         else
         {
            CancelButtonClicked.Invoke(this, new EventArgs());
            return false;
         }
      }

      public virtual void Open(FileName fileName)
      {
         _file = fileName.Some();
         if (_file.If(out var file))
         {
            textBox.Text = file.Lines.ToString("\r\n");
         }

         Clean();
      }

      public virtual void DisplayFileName()
      {
         if (displayFileName)
         {
            var title = new StringBuilder();
            if (_file.If(out var file))
            {
               title.Append(file);
               title.Append(" - ");
               title.Append(formName);
               if (IsDirty)
               {
                  title.Append(" *");
               }

               form.Text = title.ToString();
            }
            else
            {
               form.Text = formName;
            }
         }
      }

      protected string getText() => GetWindowsText(textBox.Text);

      protected void setText(string text) => textBox.Lines = SetWindowsText(text);

      public virtual void Save()
      {
         if (IsDirty)
         {
            if (_file.IsSome)
            {
               save();
            }
            else
            {
               SaveAs();
            }
         }
      }

      protected virtual void save()
      {
         if (_file.If(out var file))
         {
            if (file.Exists())
            {
               file.Delete();
            }

            file.Encoding = Encoding.UTF8;
            file.Text = getText();
         }

         Clean();
      }

      public virtual void SaveAs()
      {
         if (saveFileDialog.ShowDialog() == DialogResult.OK)
         {
            OKButtonClicked?.Invoke(this, new EventArgs());
            _file = ((FileName)saveFileDialog.FileName).Some();
            save();
         }
         else
         {
            CancelButtonClicked?.Invoke(this, new EventArgs());
         }
      }

      public virtual void Close(FormClosingEventArgs e)
      {
         if (IsDirty)
         {
            var result = getSaveResponse();
            switch (result)
            {
               case DialogResult.Yes:
                  YesButtonClicked?.Invoke(this, new EventArgs());
                  Save();
                  break;
               case DialogResult.Cancel:
                  CancelButtonClicked?.Invoke(this, new EventArgs());
                  e.Cancel = true;
                  break;
            }
         }
      }

      protected DialogResult getSaveResponse()
      {
         var message = _file.Map(f => $"File {f} not saved").DefaultTo(() => "File not saved");
         var text = $"{documentName} File Not Saved";

         return MessageBox.Show(message, text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
      }

      public virtual void Undo() => textBox.Undo();

      public virtual void Redo()
      {
         if (textBox.CanRedo)
         {
            textBox.Redo();
         }
      }

      public virtual void Cut() => textBox.Cut();

      public virtual void Copy() => textBox.Copy();

      public virtual void Paste()
      {
         var clipFormat = DataFormats.GetFormat(DataFormats.Text);
         if (textBox.CanPaste(clipFormat))
         {
            textBox.Paste(clipFormat);
         }
      }

      public virtual void Delete()
      {
         if (textBox.SelectionLength > 0)
         {
            textBox.SelectedText = "";
         }
      }

      public virtual void SelectAll() => textBox.SelectAll();

      public void CopyTextBoxSettings(RichTextBox otherTextBox)
      {
         otherTextBox.AcceptsTab = textBox.AcceptsTab;
         otherTextBox.DetectUrls = textBox.DetectUrls;
         otherTextBox.Font = new Font(textBox.Font, FontStyle.Regular);
         otherTextBox.HideSelection = textBox.HideSelection;
         otherTextBox.ShowSelectionMargin = textBox.ShowSelectionMargin;
         otherTextBox.WordWrap = textBox.WordWrap;
         otherTextBox.ScrollBars = textBox.ScrollBars;
      }

      public void CopyTextBoxSettings(TextBox otherTextBox)
      {
         otherTextBox.AcceptsTab = textBox.AcceptsTab;
         otherTextBox.Font = new Font(textBox.Font, FontStyle.Regular);
         otherTextBox.HideSelection = textBox.HideSelection;
         otherTextBox.WordWrap = textBox.WordWrap;
         switch (textBox.ScrollBars)
         {
            case RichTextBoxScrollBars.None:
               otherTextBox.ScrollBars = ScrollBars.None;
               break;
            case RichTextBoxScrollBars.Horizontal:
            case RichTextBoxScrollBars.ForcedHorizontal:
               otherTextBox.ScrollBars = ScrollBars.Horizontal;
               break;
            case RichTextBoxScrollBars.Vertical:
            case RichTextBoxScrollBars.ForcedVertical:
               otherTextBox.ScrollBars = ScrollBars.Vertical;
               break;
            case RichTextBoxScrollBars.Both:
            case RichTextBoxScrollBars.ForcedBoth:
               otherTextBox.ScrollBars = ScrollBars.Both;
               break;
         }
      }

      public string Text
      {
         get => getText();
         set => setText(value);
      }

      public string[] Lines
      {
         get => textBox.Lines;
         set => textBox.Lines = value;
      }

      public DocumentTrying TryTo => new(this);
   }
}