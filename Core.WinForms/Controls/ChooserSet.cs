using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Collections;
using Core.Monads;

namespace Core.WinForms.Controls;

public class ChooserSet
{
   protected Chooser chooser;

   internal ChooserSet(Chooser chooser)
   {
      this.chooser = chooser;
   }

   public ChooserSet Title(string title)
   {
      chooser.Title = title;
      return this;
   }

   public ChooserSet SizeToText(bool sizeToText)
   {
      chooser.SizeToText = sizeToText;
      return this;
   }

   public ChooserSet Choices(StringHash choices)
   {
      chooser.Choices = choices;
      return this;
   }

   public ChooserSet Choices(params string[] choices)
   {
      chooser.Choices = choices.ToStringHash(c => c, c => c, true);
      return this;
   }

   public ChooserSet Choices(params (string key, string value)[] choices)
   {
      chooser.Choices = choices.ToStringHash(true);
      return this;
   }

   public ChooserSet Choices(IEnumerable<string> choices) => Choices(choices.ToArray());

   public ChooserSet ForeColor(Color foreColor)
   {
      chooser.ChoiceForeColor = foreColor;
      return this;
   }

   public ChooserSet BackColor(Color backColor)
   {
      chooser.ChoiceBackColor = backColor;
      return this;
   }

   public ChooserSet NilItem(Maybe<string> _firstItem)
   {
      chooser.NilItem = _firstItem;
      return this;
   }

   public ChooserSet ModifyTitle(bool modifyTitle)
   {
      chooser.ModifyTitle = modifyTitle;
      return this;
   }

   public ChooserSet EmptyTitle(string emptyTitle)
   {
      chooser.EmptyTitle = emptyTitle;
      return this;
   }

   public ChooserSet Working(bool working = true)
   {
      chooser.Working = working;
      return this;
   }

   public Maybe<Chosen> Choose()
   {
      var _chosen = chooser.Get();
      if (chooser.ModifyTitle)
      {
         if (_chosen is (true, var chosen))
         {
            chooser.UiAction.Success(chosen.Key);
            if (chooser.SizeToText)
            {
               chooser.UiAction.SizeToText();
            }
         }
         else
         {
            chooser.UiAction.Message(chooser.EmptyTitle);
         }
      }

      return _chosen;
   }
}