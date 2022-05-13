using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using static Core.Strings.StringFunctions;

namespace Core.WinForms.Controls
{
   public class Responder : UserControl, IHash<string, Responder.ResponderButton>
   {
      public enum ResponderPersonality
      {
         Positive,
         Negative,
         Neutral,
         Critical,
         Failed
      }

      public class ResponderButton : MessageProgress
      {
         public static ResponderButton FromText(Form form, string specifier)
         {
            if (specifier.Matches(@"^ /(['!?.$']?) /(-['|']+) '|' /s* /(.+) $; f").If(out var result))
            {
               var personalityShortcut = result.FirstGroup;
               var label = result.SecondGroup.TrimEnd();
               var key = result.ThirdGroup.Trim();
               var personality = personalityShortcut switch
               {
                  "." => ResponderPersonality.Neutral,
                  "!" => ResponderPersonality.Positive,
                  "?" => ResponderPersonality.Negative,
                  "$" => ResponderPersonality.Critical,
                  _ => ResponderPersonality.Neutral
               };

               return new ResponderButton(form, personality, label, key, true);
            }
            else
            {
               return new ResponderButton(form, ResponderPersonality.Failed, $"{specifier}?", uniqueID(), true);
            }
         }

         protected string label;

         public ResponderButton(Form form, ResponderPersonality personality, string label, string key, bool center = false, bool is3D = true) :
            base(form, center, is3D)
         {
            Personality = personality;
            this.label = label;
            Key = key;
         }

         public ResponderPersonality Personality { get; set; }

         public string Label => label;

         public string Key { get; }
      }

      protected StringHash<ResponderButton> responderButtons;

      public event EventHandler<ResponderButtonArgs> ButtonClick;

      public Responder(Form form, params string[] buttonSpecifiers)
      {
         form.Controls.Add(this);

         PositiveForeColor = Color.White;
         PositiveBackColor = Color.Green;
         NegativeForeColor = Color.Black;
         NegativeBackColor = Color.Gold;
         NeutralForeColor = Color.White;
         NeutralBackColor = Color.Gray;
         CriticalForeColor = Color.White;
         CriticalBackColor = Color.Red;
         FailedForeColor = Color.Red;
         FailedBackColor = Color.White;

         responderButtons = buttonSpecifiers.Select(specifier => ResponderButton.FromText(form, specifier)).ToStringHash(rb => rb.Key, true);

         foreach (var (_, responderButton) in responderButtons)
         {
            var foreColor = responderButton.Personality switch
            {
               ResponderPersonality.Positive => PositiveForeColor,
               ResponderPersonality.Negative => NegativeForeColor,
               ResponderPersonality.Critical => CriticalForeColor,
               ResponderPersonality.Failed => FailedForeColor,
               _ => NeutralForeColor
            };
            responderButton.SetForeColor(foreColor);
            var backColor = responderButton.Personality switch
            {
               ResponderPersonality.Positive => PositiveBackColor,
               ResponderPersonality.Negative => NegativeBackColor,
               ResponderPersonality.Critical => CriticalBackColor,
               ResponderPersonality.Failed => FailedBackColor,
               _ => NeutralBackColor
            };
            responderButton.SetBackColor(backColor);
            Controls.Add(responderButton);
         }
      }

      protected void setUpButtons(int buttonHeight, string fontName, float fontSize)
      {
         var top = (Height - buttonHeight) / 2;
         var buttonsCount = responderButtons.Count;
         var padding = (buttonsCount + 1) * 2;
         var space = Width - padding;
         var width = space / buttonsCount;

         var left = 2;
         foreach (var key in responderButtons.Keys)
         {
            var button = responderButtons[key];
            button.SetUp(left, top, width, buttonHeight, AnchorStyles.Left | AnchorStyles.Top, fontName, fontSize);
            button.Message(button.Label);
            button.Click += (_, _) => { ButtonClick?.Invoke(this, new ResponderButtonArgs(key)); };
            button.ClickText = button.Label;
            left += width + 2;
         }
      }

      protected void setUpDimensions(int x, int y, int width, int height, int buttonHeight, string fontName, float fontSize)
      {
         AutoSize = false;
         Location = new Point(x, y);
         Size = new Size(width, height);

         setUpButtons(buttonHeight, fontName, fontSize);
      }

      public void SetUp(int x, int y, int width, int height, int buttonHeight, AnchorStyles anchor, string fontName = "Consolas",
         float fontSize = 10f)
      {
         setUpDimensions(x, y, width, height, buttonHeight, fontName, fontSize);
         Anchor = anchor;
      }

      public void SetUp(int x, int y, int width, int height, int buttonHeight, DockStyle dock, string fontName = "Consolas", float fontSize = 10f)
      {
         setUpDimensions(x, y, width, height, buttonHeight, fontName, fontSize);
         Dock = dock;
      }

      public void SetUpInTableLayoutPanel(TableLayoutPanel tableLayoutPanel, int buttonHeight, int column, int row, int columnSpan = 1,
         int rowSpan = 1, string fontName = "Consolas", float fontSize = 10f, DockStyle dockStyle = DockStyle.Fill)
      {
         Dock = dockStyle;
         tableLayoutPanel.Controls.Add(this, column, row);

         if (columnSpan > 1)
         {
            tableLayoutPanel.SetColumnSpan(this, columnSpan);
         }

         if (rowSpan > 1)
         {
            tableLayoutPanel.SetRowSpan(this, rowSpan);
         }

         setUpButtons(buttonHeight, fontName, fontSize);
      }

      public void SetUp(int x, int y, int width, int height, int buttonHeight, string fontName = "Consolas", float fontSize = 10f)
      {
         setUpDimensions(x, y, width, height, buttonHeight, fontName, fontSize);
      }

      public Color PositiveForeColor { get; set; }

      public Color PositiveBackColor { get; set; }

      public Color NegativeForeColor { get; set; }

      public Color NegativeBackColor { get; set; }

      public Color NeutralForeColor { get; set; }

      public Color NeutralBackColor { get; set; }

      public Color CriticalForeColor { get; set; }

      public Color CriticalBackColor { get; set; }

      public Color FailedForeColor { get; set; }

      public Color FailedBackColor { get; set; }

      public ResponderButton this[string key] => responderButtons[key];

      public bool ContainsKey(string key) => responderButtons.ContainsKey(key);

      public Result<Hash<string, ResponderButton>> AnyHash() => responderButtons;
   }
}