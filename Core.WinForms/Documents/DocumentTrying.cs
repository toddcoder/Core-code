using System.Windows.Forms;
using Core.Computers;
using Core.Monads;

namespace Core.WinForms.Documents
{
   public class DocumentTrying
   {
      protected Document document;

      public DocumentTrying(Document document) => this.document = document;

      public Document Document => document;

      public IResult<Unit> RenderMainMenu() => AttemptFunctions.tryTo(() => document.RenderMainMenu());

      public IResult<Unit> RenderContextMenu() => AttemptFunctions.tryTo(() => document.RenderContextMenu());

      public IResult<Unit> RenderContextMenu(Control control) => AttemptFunctions.tryTo(() => document.RenderContextMenu(control));

      public IResult<Unit> New() => AttemptFunctions.tryTo(() => document.New());

      public IResult<Unit> Open() => AttemptFunctions.tryTo(() => document.Open());

      public IResult<Unit> Open(FileName fileName) => AttemptFunctions.tryTo(() => document.Open(fileName));

      public IResult<Unit> Save() => AttemptFunctions.tryTo(() => document.Save());

      public IResult<Unit> SaveAs() => AttemptFunctions.tryTo(() => document.SaveAs());

      public IResult<Unit> Close(FormClosingEventArgs e) => AttemptFunctions.tryTo(() => document.Close(e));
   }
}