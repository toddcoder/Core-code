namespace Core.WinForms.Controls;

public class UiActionCheckToggler
{
   protected UiAction[] uiActions;

   public UiActionCheckToggler(params UiAction[] uiActions)
   {
      this.uiActions = new UiAction[uiActions.Length];

      for (var i = 0; i < uiActions.Length; i++)
      {
         this.uiActions[i] = uiActions[i];
         this.uiActions[i].CheckStyleChanged += (_, e) => checkChanged(e);
      }
   }

   protected void checkChanged(CheckStyleChangedArgs e)
   {
      if (e.CheckStyle == CheckStyle.Checked)
      {
         /*for (var i = 0; i < uiActions.Length && uiActions[i].Id != e.Id; i++)
         {
            uiActions[i].SetCheckStyle(CheckStyle.Unchecked);
         }*/
         foreach (var uiAction in uiActions)
         {
            if (uiAction.Id != e.Id)
            {
               uiAction.SetCheckStyle(CheckStyle.Unchecked);
            }
         }
      }
   }
}