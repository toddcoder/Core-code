using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Collections;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ShiftKey
{
   protected const string SHIFT_CTL = "Ý"; // wingdings2
   protected const string SHIFT_ALT = "U"; //wingdings3
   protected const string SHIFT_SHIFT = "X"; //wingdings3
   protected const string SHIFT_WIN = "\u0255"; //wingdings

   protected UiAction uiAction;
   protected string defaultMessage;
   protected CardinalAlignment alignment;
   protected Hash<ShiftKeyStatus, string> messages;
   protected Maybe<SubText> _subText;
   protected ShiftKeyStatus lastStatus;
   protected string message;
   protected Maybe<string> _prefix;
   protected Maybe<string> _fontName;

   public ShiftKey(UiAction uiAction, string defaultMessage, TimeSpan elapsedTime, CardinalAlignment alignment = CardinalAlignment.NorthEast)
   {
      this.uiAction = uiAction;
      this.defaultMessage = defaultMessage;
      this.alignment = alignment;

      this.uiAction.Tick += (_, _) => this.uiAction.Refresh();
      this.uiAction.PaintingBackground += (_, e) => e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
      this.uiAction.Painting += (_, e) => ShowMessage(e.Graphics, e.ClipRectangle);
      this.uiAction.StartTimer(elapsedTime);

      messages = new Hash<ShiftKeyStatus, string>();
      _subText = nil;
      lastStatus = ShiftKeyStatus.None;

      message = defaultMessage;
      _prefix = nil;
      _fontName = nil;
   }

   public ShiftKey(UiAction uiAction, string defaultMessage, CardinalAlignment alignment = CardinalAlignment.NorthEast) :
      this(uiAction, defaultMessage, 500.Milliseconds(), alignment)
   {
   }

   public string this[ShiftKeyStatus status]
   {
      get => messages.Maybe[status] | defaultMessage;
      set => messages[status] = value;
   }

   public ShiftKeyStatus Status
   {
      get
      {
         var modifierKeys = Control.ModifierKeys;
         return modifierKeys switch
         {
            Keys.ControlKey => ShiftKeyStatus.Control,
            Keys.Alt => ShiftKeyStatus.Alt,
            Keys.ShiftKey => ShiftKeyStatus.Shift,
            Keys.LWin => ShiftKeyStatus.Windows,
            _ => ShiftKeyStatus.None
         };
      }
   }

   protected Maybe<string> getPrefix(ShiftKeyStatus status) => status switch
   {
      ShiftKeyStatus.Control => SHIFT_CTL,
      ShiftKeyStatus.Alt => SHIFT_ALT,
      ShiftKeyStatus.Shift => SHIFT_SHIFT,
      ShiftKeyStatus.Windows => SHIFT_WIN,
      _ => nil
   };

   protected Maybe<string> getFontName(ShiftKeyStatus status) => status switch
   {
      ShiftKeyStatus.Control => "Wingdings 2",
      ShiftKeyStatus.Alt or ShiftKeyStatus.Shift => "Wingdings 3",
      ShiftKeyStatus.Windows => "Wingdings",
      _ => nil
   };

   public void ShowMessage(Graphics g, Rectangle rectangle)
   {
      var status = Status;
      if (status != lastStatus)
      {
         message = this[status];
         _prefix = getPrefix(status);
         _fontName = getFontName(status);
         lastStatus = status;
      }

      using var textFont = new Font("Consolas", 8);
      if (_prefix is (true, var prefix) && _fontName is (true, var fontName))
      {
         using var prefixFont = new Font(fontName, 8);
         showMessage(g, prefix, prefixFont, textFont, rectangle);
      }
      else
      {
         showMessage(g, " ", textFont, textFont, rectangle);
      }
   }

   protected void showMessage(Graphics g, string prefix, Font prefixFont, Font textFont, Rectangle rectangle)
   {
      var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
      var prefixSize = UiActionWriter.TextSize(g, prefix, prefixFont, flags);
      var textSize = UiActionWriter.TextSize(g, message, textFont, flags);
      var maxHeight = prefixSize.Height.MaxOf(textSize.Height);
      var width = prefixSize.Width + 3 + textSize.Width;
   }
}