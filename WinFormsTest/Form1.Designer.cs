namespace WinFormsTest
{
   partial class Form1
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
         this.button1 = new System.Windows.Forms.Button();
         this.button2 = new System.Windows.Forms.Button();
         this.button3 = new System.Windows.Forms.Button();
         this.panel1 = new System.Windows.Forms.Panel();
         this.imageList1 = new System.Windows.Forms.ImageList(this.components);
         this.panel2 = new System.Windows.Forms.Panel();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.panel3 = new System.Windows.Forms.Panel();
         this.panel4 = new System.Windows.Forms.Panel();
         this.SuspendLayout();
         // 
         // button1
         // 
         this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.button1.Location = new System.Drawing.Point(713, 12);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(75, 23);
         this.button1.TabIndex = 0;
         this.button1.Text = "button1";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.button1_Click);
         // 
         // button2
         // 
         this.button2.Location = new System.Drawing.Point(713, 41);
         this.button2.Name = "button2";
         this.button2.Size = new System.Drawing.Size(75, 23);
         this.button2.TabIndex = 1;
         this.button2.Text = "button2";
         this.button2.UseVisualStyleBackColor = true;
         this.button2.Click += new System.EventHandler(this.button2_Click);
         // 
         // button3
         // 
         this.button3.Location = new System.Drawing.Point(713, 70);
         this.button3.Name = "button3";
         this.button3.Size = new System.Drawing.Size(75, 23);
         this.button3.TabIndex = 2;
         this.button3.Text = "button3";
         this.button3.UseVisualStyleBackColor = true;
         this.button3.Click += new System.EventHandler(this.button3_Click);
         // 
         // panel1
         // 
         this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.panel1.Location = new System.Drawing.Point(12, 41);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(695, 40);
         this.panel1.TabIndex = 3;
         // 
         // imageList1
         // 
         this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
         this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
         this.imageList1.Images.SetKeyName(0, "AbstractAssociation.png");
         // 
         // panel2
         // 
         this.panel2.Location = new System.Drawing.Point(12, 99);
         this.panel2.Name = "panel2";
         this.panel2.Size = new System.Drawing.Size(20, 20);
         this.panel2.TabIndex = 4;
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(38, 99);
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(750, 20);
         this.textBox1.TabIndex = 5;
         // 
         // panel3
         // 
         this.panel3.Location = new System.Drawing.Point(12, 125);
         this.panel3.Name = "panel3";
         this.panel3.Size = new System.Drawing.Size(776, 20);
         this.panel3.TabIndex = 6;
         // 
         // panel4
         // 
         this.panel4.Location = new System.Drawing.Point(12, 151);
         this.panel4.Name = "panel4";
         this.panel4.Size = new System.Drawing.Size(776, 31);
         this.panel4.TabIndex = 7;
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(800, 450);
         this.Controls.Add(this.panel4);
         this.Controls.Add(this.panel3);
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.panel2);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.button3);
         this.Controls.Add(this.button2);
         this.Controls.Add(this.button1);
         this.Name = "Form1";
         this.Text = "Form1";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Button button2;
      private System.Windows.Forms.Button button3;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.ImageList imageList1;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.TextBox textBox1;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Panel panel4;
   }
}

