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

      public IResult<Unit> RenderMainMenu() => tryTo(() => document.RenderMainMenu());

      public IResult<Unit> RenderContextMenu() => tryTo(() => document.RenderContextMenu());

      public IResult<Unit> RenderContextMenu(Control control) => tryTo(() => document.RenderContextMenu(control));

      public IResult<Unit> New() => tryTo(() => document.New());

      public IResult<Unit> Open() => tryTo(() => document.Open());

      public IResult<Unit> Open(FileName fileName) => tryTo(() => document.Open(fileName));

      public IResult<Unit> Save() => tryTo(() => document.Save());

      public IResult<Unit> SaveAs() => tryTo(() => document.SaveAs());

      public IResult<Unit> Close(FormClosingEventArgs e) => tryTo(() => document.Close(e));
   }
}