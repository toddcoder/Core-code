using Core.Monads;

namespace Core.WinForms.Controls;

public class CheckBoxWriter : AlternateWriter
{
   public CheckBoxWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling) :
      base(uiAction, alternates, autoSizeText, _floor, _ceiling)
   {
   }

   public bool BoxChecked
   {
      get => selectedIndex != -1;
      set => selectedIndex = value ? 0 : -1;
   }
}