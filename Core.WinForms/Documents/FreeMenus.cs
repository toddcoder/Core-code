using System;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Documents;

public class FreeMenus : Menus
{
   public FreeMenus()
   {
      Document = nil;
      Form = nil;
      SaveAll = nil;
   }

   public Maybe<Document> Document { get; set; }

   public Maybe<Form> Form { get; set; }

   public Maybe<EventHandler> SaveAll { get; set; }

   public void StandardContextEdit()
   {
      ContextMenu("Undo", (_, _) => Document.IfThen(d => d.Undo()), "^Z");
      ContextMenu("Redo", (_, _) => Document.IfThen(d => d.Redo()));
      ContextMenuSeparator();
      ContextMenu("Cut", (_, _) => Document.IfThen(d => d.Cut()), "^X");
      ContextMenu("Copy", (_, _) => Document.IfThen(d => d.Copy()), "^C");
      ContextMenu("Paste", (_, _) => Document.IfThen(d => d.Paste()), "^V");
      ContextMenu("Delete", (_, _) => Document.IfThen(d => d.Delete()));
      ContextMenuSeparator();
      ContextMenu("Select All", (_, _) => Document.IfThen(d => d.SelectAll()), "^A");
   }

   public void StandardFileMenu()
   {
      Menu("&File");
      Menu("File", "New...", (_, _) => Document.IfThen(d => d.New()), "^N");
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
      Menu("File", "Open...", (_, _) => Document.IfThen(d => d.Open()), "^O");
      Menu("File", "Save", (_, _) => Document.IfThen(d => d.Save()), "^S");
      Menu("File", "Save As...", (_, _) => Document.IfThen(d => d.SaveAs()));
      SaveAll.IfThen(eh => Menu("File", "Save All", eh, "^|S"));
      MenuSeparator("File");
      Menu("File", "Exit", (_, _) => Form.IfThen(f => f.Close()), "%F4");
   }

   public void StandardEditMenu()
   {
      Menu("&Edit");
      Menu("Edit", "Undo", (_, _) => Document.IfThen(d => d.Undo()), "^Z");
      Menu("Edit", "Redo", (_, _) => Document.IfThen(d => d.Redo()));
      MenuSeparator("Edit");
      Menu("Edit", "Cut", (_, _) => Document.IfThen(d => d.Cut()), "^X");
      Menu("Edit", "Copy", (_, _) => Document.IfThen(d => d.Copy()), "^C");
      Menu("Edit", "Paste", (_, _) => Document.IfThen(d => d.Paste()), "^V");
      Menu("Edit", "Delete", (_, _) => Document.IfThen(d => d.Delete()));
      MenuSeparator("Edit");
      Menu("Edit", "Select All", (_, _) => Document.IfThen(d => d.SelectAll()), "^A");
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