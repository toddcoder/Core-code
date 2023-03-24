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

   public Optional<Document> Document { get; set; }

   public Optional<Form> Form { get; set; }

   public Optional<EventHandler> SaveAll { get; set; }

   public void StandardContextEdit()
   {
      ContextMenu("Undo", (_, _) =>
      {
         if (Document is (true, var document))
         {
            document.Undo();
         }
      }, "^Z");
      ContextMenu("Redo", (_, _) =>
      {
         if (Document is (true, var document))
         {
            document.Redo();
         }
      });
      ContextMenuSeparator();
      ContextMenu("Cut", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Cut();
         }
      }, "^X");
      ContextMenu("Copy", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Copy();
         }
      }, "^C");
      ContextMenu("Paste", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Paste();
         }
      }, "^V");
      ContextMenu("Delete", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Delete();
         }
      });
      ContextMenuSeparator();
      ContextMenu("Select All", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.SelectAll();
         }
      }, "^A");
   }

   public void StandardFileMenu()
   {
      Menu("&File");
      Menu("File", "New...", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.New();
         }
      }, "^N");
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
      Menu("File", "Open...", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Open();
         }
      }, "^O");
      Menu("File", "Save", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Save();
         }
      }, "^S");
      Menu("File", "Save As...", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.SaveAs();
         }
      });
      if (SaveAll is (true, var saveAll))
      {
         Menu("File", "Save All", saveAll, "^|S");
      }
      MenuSeparator("File");
      Menu("File", "Exit", (_, _) =>
      {
         if (Form is (true, var form))
         {
            form.Close();
         }
      }, "%F4");
   }

   public void StandardEditMenu()
   {
      Menu("&Edit");
      Menu("Edit", "Undo", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Undo();
         }
      }, "^Z");
      Menu("Edit", "Redo", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Redo();
         }
      });
      MenuSeparator("Edit");
      Menu("Edit", "Cut", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Cut();
         }
      }, "^X");
      Menu("Edit", "Copy", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Copy();
         }
      }, "^C");
      Menu("Edit", "Paste", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Paste();
         }
      }, "^V");
      Menu("Edit", "Delete", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.Delete();
         }
      });
      MenuSeparator("Edit");
      Menu("Edit", "Select All", (_, _) =>
      {
         if (Document is (true, var d))
         {
            d.SelectAll();
         }
      }, "^A");
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

   public void RenderMainMenu()
   {
      if (Form is (true, var form))
      {
         CreateMainMenu(form);
      }
   }
}