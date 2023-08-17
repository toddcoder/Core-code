using Core.Dates.DateIncrements;
using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class KeyMatch
{
   public static implicit operator bool(KeyMatch keyMatch) => keyMatch.IsDown;

   public static bool operator true(KeyMatch keyMatch) => keyMatch.IsDown;

   public static bool operator false(KeyMatch keyMatch) => !keyMatch.IsDown;

   protected Keys keys;
   protected UiAction uiAction;
   protected string downMessage;
   protected string upMessage;
   protected bool isDown;
   protected Maybe<SubText> _subText;

   public KeyMatch(Keys keys, UiAction uiAction, string downMessage, string upMessage, TimeSpan elapsedTime)
   {
      this.keys = keys;
      this.uiAction = uiAction;
      this.downMessage = downMessage;
      this.upMessage = upMessage;

      Alignment = CardinalAlignment.NorthEast;
      isDown = false;
      _subText = nil;

      this.uiAction.Tick += (_, _) => ShowMessage();
      this.uiAction.StartTimer(elapsedTime);
      DisplayUp();
   }

   public KeyMatch(Keys keys, UiAction uiAction, string downMessage, string upMessage) : this(keys, uiAction, downMessage, upMessage,
      500.Milliseconds())
   {
   }

   public bool IsDown => isDown;

   public CardinalAlignment Alignment { get; set; }

   public KeyMatchStatus Status()
   {
      if (Control.ModifierKeys == keys && !isDown)
      {
         isDown = true;
         return KeyMatchStatus.Down;
      }
      else if (Control.ModifierKeys != keys && isDown)
      {
         isDown = false;
         return KeyMatchStatus.Up;
      }
      else
      {
         return KeyMatchStatus.Ignore;
      }
   }

   public void ClearMessage()
   {
      if (_subText is (true, var subText))
      {
         uiAction.RemoveSubText(subText);
         _subText = nil;
      }
   }

   public void DisplayMessage(string message)
   {
      Maybe<string> getPrefix() => keys switch
      {
         Keys.Control => "Ⓒ",
         Keys.Shift => "Ⓢ",
         Keys.Alt => "Ⓐ",
         _ => nil
      };

      var fullMessage = getPrefix().Map(p => $"{p}/2bar.{message}");
      _subText = uiAction.SubText(fullMessage).Set.Alignment(Alignment).FontSize(8).FontStyle(FontStyle.Bold).ForeColor(Color.Black)
         .BackColor(Color.White).Outline().IncludeFloor(false).IncludeCeiling(false).Outline().End;
      uiAction.Refresh();
   }

   public void DisplayDown() => DisplayMessage(downMessage);

   public void DisplayUp() => DisplayMessage(upMessage);

   public void ShowMessage()
   {
      switch (Status())
      {
         case KeyMatchStatus.Down:
            ClearMessage();
            DisplayDown();
            uiAction.Refresh();
            break;
         case KeyMatchStatus.Up:
            ClearMessage();
            DisplayUp();
            uiAction.Refresh();
            break;
      }
   }
}