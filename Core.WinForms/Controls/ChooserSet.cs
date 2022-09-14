using System.Drawing;
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

   public Maybe<Chosen> Choose()
   {
      var _chosen = chooser.Get();
      if (chooser.ModifyTitle)
      {
         if (_chosen.Map(out var chosen))
         {
            chooser.UiAction.Success(chosen.Key);
         }
         else
         {
            chooser.UiAction.Message(chooser.EmptyTitle);
         }
      }

      return _chosen;
   }
}