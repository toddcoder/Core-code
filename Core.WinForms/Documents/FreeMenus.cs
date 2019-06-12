using System;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Documents
{
	public class FreeMenus : Menus
	{
		public IMaybe<Document> Document { get; set; } = none<Document>();

		public IMaybe<Form> Form { get; set; } = none<Form>();

		public IMaybe<EventHandler> SaveAll { get; set; } = none<EventHandler>();

		public void StandardContextEdit()
		{
			ContextMenu("Undo", (sender, e) => Document.IfThen(d => d.Undo()), "^Z");
			ContextMenu("Redo", (sender, e) => Document.IfThen(d => d.Redo()));
			ContextMenuSeparator();
			ContextMenu("Cut", (sender, e) => Document.IfThen(d => d.Cut()), "^X");
			ContextMenu("Copy", (sender, e) => Document.IfThen(d => d.Copy()), "^C");
			ContextMenu("Paste", (sender, e) => Document.IfThen(d => d.Paste()), "^V");
			ContextMenu("Delete", (sender, e) => Document.IfThen(d => d.Delete()));
			ContextMenuSeparator();
			ContextMenu("Select All", (sender, e) => Document.IfThen(d => d.SelectAll()), "^A");
		}

		public void StandardFileMenu()
		{
			Menu("&File");
			Menu("File", "New...", (sender, e) => Document.IfThen(d => d.New()), "^N");
			standardItems();
		}

		public void StandardFileMenu(EventHandler handler)
		{
			Menu("&File");
			Menu("File", "New", handler, "^N");
			standardItems();
		}

		protected void standardItems()
		{
			Menu("File", "Open...", (sender, e) => Document.IfThen(d => d.Open()), "^O");
			Menu("File", "Save", (sender, e) => Document.IfThen(d => d.Save()), "^S");
			Menu("File", "Save As...", (sender, e) => Document.IfThen(d => d.SaveAs()));
			SaveAll.IfThen(eh => Menu("File", "Save All", eh, "^|S"));
			MenuSeparator("File");
			Menu("File", "Exit", (sender, e) => Form.IfThen(f => f.Close()), "%F4");
		}

		public void StandardEditMenu()
		{
			Menu("&Edit");
			Menu("Edit", "Undo", (sender, e) => Document.IfThen(d => d.Undo()), "^Z");
			Menu("Edit", "Redo", (sender, e) => Document.IfThen(d => d.Redo()));
			MenuSeparator("Edit");
			Menu("Edit", "Cut", (sender, e) => Document.IfThen(d => d.Cut()), "^X");
			Menu("Edit", "Copy", (sender, e) => Document.IfThen(d => d.Copy()), "^C");
			Menu("Edit", "Paste", (sender, e) => Document.IfThen(d => d.Paste()), "^V");
			Menu("Edit", "Delete", (sender, e) => Document.IfThen(d => d.Delete()));
			MenuSeparator("Edit");
			Menu("Edit", "Select All", (sender, e) => Document.IfThen(d => d.SelectAll()), "^A");
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

		public void RenderMainMenu() => Form.IfThen(CreateMainMenu);
	}
}