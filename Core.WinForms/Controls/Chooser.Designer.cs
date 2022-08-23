namespace Core.WinForms.Controls
{
   partial class Chooser
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.listViewItems = new System.Windows.Forms.ListView();
         this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.SuspendLayout();
         // 
         // listViewItems
         // 
         this.listViewItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.listViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listViewItems.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.listViewItems.FullRowSelect = true;
         this.listViewItems.GridLines = true;
         this.listViewItems.HideSelection = false;
         this.listViewItems.Location = new System.Drawing.Point(0, 0);
         this.listViewItems.Name = "listViewItems";
         this.listViewItems.Scrollable = false;
         this.listViewItems.Size = new System.Drawing.Size(400, 450);
         this.listViewItems.TabIndex = 0;
         this.listViewItems.UseCompatibleStateImageBehavior = false;
         this.listViewItems.View = System.Windows.Forms.View.Details;
         this.listViewItems.SelectedIndexChanged += new System.EventHandler(this.listViewItems_SelectedIndexChanged);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "";
         this.columnHeader1.Width = 25;
         // 
         // Chooser
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(400, 450);
         this.Controls.Add(this.listViewItems);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Chooser";
         this.Load += new System.EventHandler(this.Chooser_Load);
         this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Chooser_MouseDown);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ListView listViewItems;
      private System.Windows.Forms.ColumnHeader columnHeader1;
   }
}