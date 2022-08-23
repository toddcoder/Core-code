using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls
{
   public partial class Chooser : Form
   {
      public static Maybe<string> Get(string title, IEnumerable<string> choices)
      {
         using var chooser = new Chooser(title, choices);
         chooser.ShowDialog();
         return chooser.Choice;
      }

      protected string title;
      protected StringSet choices;

      public Chooser(string title, IEnumerable<string> choices)
      {
         this.title = title;
         this.choices = new StringSet(true, choices);

         InitializeComponent();

         Choice = nil;
      }

      public Maybe<string> Choice { get; set; }

      protected void addItem(string text, Color foreColor, Color backColor)
      {
         var item = listViewItems.Items.Add(text);
         item.UseItemStyleForSubItems = true;
         item.ForeColor = foreColor;
         item.BackColor = backColor;
      }

      protected void Chooser_Load(object sender, EventArgs e)
      {
         Location = Cursor.Position;
         addItem("none", Color.Black, Color.Gold);

         foreach (var choice in choices)
         {
            addItem(choice, Color.White, Color.Green);
         }

         listViewItems.Columns[0].Width = ClientSize.Width;
         listViewItems.Columns[0].Text = title;

         var lastItem = listViewItems.Items[listViewItems.Items.Count - 1];
         var bounds = lastItem.Bounds;
         var bottom = bounds.Bottom;
         Height = bottom + 4;
      }

      protected void Chooser_MouseDown(object sender, MouseEventArgs e)
      {
         if (!ClientRectangle.Contains(Cursor.Position))
         {
            Close();
         }
      }

      protected void listViewItems_SelectedIndexChanged(object sender, EventArgs e)
      {
         Choice = listViewItems.SelectedText().Map(t => t.index > 0 & t.text.Some());
         Close();
      }
   }
}