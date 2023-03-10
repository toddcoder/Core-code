namespace Core.WinForms.Documents;

public static class MenuBuilderFunctions
{
   public static MenuBuilder.MenuEnd menu => new();

   public static MenuBuilder.BuilderSubMenu subMenu => new();

   public static MenuBuilder.BuilderIsChecked isChecked => new();
}