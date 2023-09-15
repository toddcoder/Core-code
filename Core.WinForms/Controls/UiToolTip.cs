using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using Timer = System.Timers.Timer;

namespace Core.WinForms.Controls;

public class UiToolTip : ToolTip
{
   protected UiAction uiAction;
   protected Font font;
   protected Maybe<Action<object, DrawToolTipEventArgs>> _action;
   protected TextFormatFlags textFormatFlags;
   protected Timer timer;

   public UiToolTip(UiAction uiAction)
   {
      this.uiAction = uiAction;
      font = this.uiAction.Font;
      Text = string.Empty;

      _action = nil;
      textFormatFlags = TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.HorizontalCenter |
         TextFormatFlags.NoClipping;
      timer = new Timer { Interval = 10000 };
      timer.Elapsed += (_, _) =>
      {
         this.uiAction.ClearFloating();
         timer.Stop();
      };

      OwnerDraw = true;
      IsBalloon = false;
      Popup += onPopUp;
      Draw += onDraw;
   }

   public Font Font
   {
      get => font;
      set => font = value;
   }

   public Maybe<Action<object, DrawToolTipEventArgs>> Action
   {
      get => _action;
      set => _action = value;
   }

   public bool ToolTipBox { get; set; }

   public string Text { get; set; }

   protected void onPopUp(object sender, PopupEventArgs e)
   {
      Size getTextSize()
      {
         var size = TextRenderer.MeasureText(Text, font, Size.Empty, textFormatFlags);
         var extraHeight = 0;
         if (ToolTipTitle.IsNotEmpty())
         {
            extraHeight += 20;
         }

         if (uiAction.FailureToolTip || uiAction.ExceptionToolTip)
         {
            extraHeight += 20;
         }

         if (extraHeight > 0)
         {
            size = size with { Height = size.Height + extraHeight };
         }

         return size;
      }

      if (uiAction.FailureToolTip is (true, var failureToolTip))
      {
         Text = failureToolTip;
         e.ToolTipSize = getTextSize();
      }
      else if (uiAction.ExceptionToolTip is (true, var exceptionToolTip))
      {
         Text = exceptionToolTip;
         e.ToolTipSize = getTextSize();
      }
      else if (uiAction._clickText is (true, var clickText) && !uiAction.IsFailureOrException)
      {
         Text = clickText;
         e.ToolTipSize = getTextSize();
      }
      else if (uiAction.HasDynamicToolTip)
      {
         var _toolTipText = uiAction.RaiseDynamicToolTip();
         if (_toolTipText is (true, var toolTipText))
         {
            Text = toolTipText;
            e.ToolTipSize = getTextSize();
         }
      }
      else
      {
         Text = uiAction.Text;
         e.ToolTipSize = getTextSize();
      }
   }

   public void DrawTextInRectangle(Graphics g, Font font, Color foreColor, Color backColor, Rectangle bounds)
   {
      using var brush = new SolidBrush(backColor);
      g.FillRectangle(brush, bounds);

      if (ToolTipTitle.IsNotEmpty())
      {
         bounds = bounds with { Y = bounds.Y + 20, Height = bounds.Height - 20 };
      }

      var writer = new UiActionWriter(CardinalAlignment.Center, false, nil, nil, UiActionButtonType.Normal)
      {
         Font = font,
         Color = foreColor,
         Rectangle = bounds,
         Flags = textFormatFlags
      };
      writer.Write(g, Text);
   }

   public void DrawTitle(Graphics g, Font font, Color foreColor, Color backColor, Rectangle bounds)
   {
      if (ToolTipTitle.IsNotEmpty())
      {
         using var smallFont = new Font(font.FontFamily, 8f);
         var smallBounds = new Rectangle(bounds.Location, bounds.Size with { Height = 20 });
         using var brush = new SolidBrush(backColor);
         g.FillRectangle(brush, smallBounds);

         var writer = new UiActionWriter(CardinalAlignment.Center, false, nil, nil, UiActionButtonType.Normal)
         {
            Font = smallFont,
            Color = foreColor,
            Rectangle = smallBounds
         };
         writer.Write(g, ToolTipTitle);
      }
   }

   protected void onDraw(object sender, DrawToolTipEventArgs e)
   {
      if (_action is (true, var action))
      {
         action(sender, e);
         timer.Start();
      }
      else
      {
         var foreColor = Color.White;
         var backColor = Color.CadetBlue;

         DrawTextInRectangle(e.Graphics, font, foreColor, backColor, e.Bounds);
         DrawTitle(e.Graphics, font, backColor, foreColor, e.Bounds);

         if (ToolTipBox)
         {
            using var pen = new Pen(foreColor);
            var bounds = e.Bounds;
            bounds.Inflate(-2, -1);
            e.Graphics.DrawRectangle(pen, bounds);
         }
      }
   }
}