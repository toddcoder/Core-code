namespace Core.WinForms.Controls;

public class ValidatedArgs
{
   public ValidatedArgs(string text)
   {
      Text = text;
      Type = UiActionType.Success;
   }

   public string Text { get; }

   public UiActionType Type { get; set; }
}