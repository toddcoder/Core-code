using System.Windows.Forms;
using Core.Computers;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.WinForms.Documents
{
   public class DocumentTrying
   {
      protected Document document;

      public DocumentTrying(Document document) => this.document = document;

      public Document Document => document;

      public Result<Unit> RenderMainMenu() => tryTo(() => document.RenderMainMenu());

      public Result<Unit> RenderContextMenu() => tryTo(() => document.RenderContextMenu());

      public Result<Unit> RenderContextMenu(Control control) => tryTo(() => document.RenderContextMenu(control));

      public Result<Unit> New() => tryTo(() => document.New());

      public Result<Unit> Open() => tryTo(() => document.Open());

      public Result<Unit> Open(FileName fileName) => tryTo(() => document.Open(fileName));

      public Result<Unit> Save() => tryTo(() => document.Save());

      public Result<Unit> SaveAs() => tryTo(() => document.SaveAs());

      public Result<Unit> Close(FormClosingEventArgs e) => tryTo(() => document.Close(e));
   }
}