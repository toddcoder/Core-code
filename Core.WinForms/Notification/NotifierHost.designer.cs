namespace Core.WinForms.Notification
{
   partial class NotifierHost
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
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifierHost));
         this.panelRight = new System.Windows.Forms.Panel();
         this.labelTitle = new System.Windows.Forms.Label();
         this.labelMessage = new System.Windows.Forms.Label();
         this.timerStyler = new System.Windows.Forms.Timer(this.components);
         this.timerCloser = new System.Windows.Forms.Timer(this.components);
         this.icons = new System.Windows.Forms.ImageList(this.components);
         this.panelLeft = new ImagePanel();
         this.panelRight.SuspendLayout();
         this.SuspendLayout();
         // 
         // panelRight
         // 
         this.panelRight.Controls.Add(this.labelTitle);
         this.panelRight.Controls.Add(this.labelMessage);
         this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panelRight.Location = new System.Drawing.Point(72, 0);
         this.panelRight.Name = "panelRight";
         this.panelRight.Size = new System.Drawing.Size(358, 69);
         this.panelRight.TabIndex = 1;
         // 
         // labelTitle
         // 
         this.labelTitle.AutoSize = true;
         this.labelTitle.Font = new System.Drawing.Font("Constantia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelTitle.Location = new System.Drawing.Point(6, 9);
         this.labelTitle.Name = "labelTitle";
         this.labelTitle.Size = new System.Drawing.Size(40, 19);
         this.labelTitle.TabIndex = 1;
         this.labelTitle.Text = "title";
         // 
         // labelMessage
         // 
         this.labelMessage.AutoSize = true;
         this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelMessage.Location = new System.Drawing.Point(6, 28);
         this.labelMessage.Name = "labelMessage";
         this.labelMessage.Size = new System.Drawing.Size(65, 16);
         this.labelMessage.TabIndex = 0;
         this.labelMessage.Text = "message";
         // 
         // timerStyler
         // 
         this.timerStyler.Interval = 1;
         this.timerStyler.Tick += new System.EventHandler(this.timerStyler_Tick);
         // 
         // timerCloser
         // 
         this.timerCloser.Tick += new System.EventHandler(this.timerCloser_Tick);
         // 
         // icons
         // 
         this.icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("icons.ImageStream")));
         this.icons.TransparentColor = System.Drawing.Color.Transparent;
         this.icons.Images.SetKeyName(0, "error");
         this.icons.Images.SetKeyName(1, "info");
         this.icons.Images.SetKeyName(2, "warning");
         // 
         // panelLeft
         // 
         this.panelLeft.BackColor = System.Drawing.Color.Transparent;
         this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
         this.panelLeft.Image = null;
         this.panelLeft.Location = new System.Drawing.Point(0, 0);
         this.panelLeft.Name = "panelLeft";
         this.panelLeft.Size = new System.Drawing.Size(72, 69);
         this.panelLeft.TabIndex = 0;
         // 
         // NotifierHost
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(430, 69);
         this.Controls.Add(this.panelRight);
         this.Controls.Add(this.panelLeft);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
         this.Name = "NotifierHost";
         this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
         this.Text = "NotifierHost";
         this.Load += new System.EventHandler(this.NotifierHost_Load);
         this.panelRight.ResumeLayout(false);
         this.panelRight.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private ImagePanel panelLeft;
      private System.Windows.Forms.Panel panelRight;
      private System.Windows.Forms.Label labelTitle;
      private System.Windows.Forms.Label labelMessage;
      private System.Windows.Forms.Timer timerStyler;
      private System.Windows.Forms.Timer timerCloser;
      private System.Windows.Forms.ImageList icons;
   }
}