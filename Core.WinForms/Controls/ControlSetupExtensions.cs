using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public static class ControlSetupExtensions
{
   private static void setUpFont(Control control, string fontName, float fontSize)
   {
      control.Font = new Font(fontName, fontSize);
   }

   private static void setUpDimensions(Control control, int x, int y, int width, int height, string fontName, float fontSize)
   {
      control.AutoSize = false;
      control.Location = new Point(x, y);
      control.Size = new Size(width, height);
      setUpFont(control, fontName, fontSize);
   }

   public static void SetUp(this Control control, int x, int y, int width, int height, AnchorStyles anchor, string fontName = "Consolas",
      float fontSize = 12f)
   {
      setUpDimensions(control, x, y, width, height, fontName, fontSize);
      control.Anchor = anchor;
   }

   public static void SetUp(this Control control, int x, int y, int width, int height, DockStyle dock, string fontName = "Consolas",
      float fontSize = 12f)
   {
      setUpDimensions(control, x, y, width, height, fontName, fontSize);
      control.Dock = dock;
   }

   public static void SetUpInTableLayoutPanel(this Control control, TableLayoutPanel tableLayoutPanel, int column, int row, int columnSpan = 1,
      int rowSpan = 1, string fontName = "Consolas", float fontSize = 12f, DockStyle dockStyle = DockStyle.Fill)
   {
      control.Dock = dockStyle;
      tableLayoutPanel.Controls.Add(control, column, row);

      if (columnSpan > 1)
      {
         tableLayoutPanel.SetColumnSpan(control, columnSpan);
      }

      if (rowSpan > 1)
      {
         tableLayoutPanel.SetRowSpan(control, rowSpan);
      }

      setUpFont(control, fontName, fontSize);
   }

   public static void SetUpInPanel(this Control control, Panel panel, string fontName = "Consolas", float fontSize = 12f,
      DockStyle dockStyle = DockStyle.Fill)
   {
      control.Dock = dockStyle;
      panel.Controls.Add(control);
      setUpFont(control, fontName, fontSize);
   }

   public static void SetUp(this Control control, int x, int y, int width, int height, string fontName = "Consolas", float fontSize = 12f)
   {
      setUpDimensions(control, x, y, width, height, fontName, fontSize);
   }

   public static void SetupInFlowLayoutPanel(this Control control, FlowLayoutPanel flowLayoutPanel, string fontName = "Consolas",
      float fontSize = 12f,DockStyle dockStyle = DockStyle.Fill)
   {
      control.Dock = dockStyle;
      flowLayoutPanel.Controls.Add(control);
      setUpFont(control, fontName, fontSize);
   }
}