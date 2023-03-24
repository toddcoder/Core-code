using System.Windows.Forms;
using Core.Computers;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.WinForms.Documents;

public class DocumentTrying
{
   protected Document document;

   public DocumentTrying(Document document) => this.document = document;

   public Document Document => document;

   public Optional<Unit> RenderMainMenu() => tryTo(() => document.RenderMainMenu());

   public Optional<Unit> RenderContextMenu() => tryTo(() => document.RenderContextMenu());

   public Optional<Unit> RenderContextMenu(Control control) => tryTo(() => document.RenderContextMenu(control));

   public Optional<Unit> New() => tryTo(() => document.New());

   public Optional<Unit> Open() => tryTo(() => document.Open());

   public Optional<Unit> Open(FileName fileName) => tryTo(() => document.Open(fileName));

   public Optional<Unit> Save() => tryTo(() => document.Save());

   public Optional<Unit> SaveAs() => tryTo(() => document.SaveAs());

   public Optional<Unit> Close(FormClosingEventArgs e) => tryTo(() => document.Close(e));
}