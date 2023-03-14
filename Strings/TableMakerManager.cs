namespace Core.Strings;

public static class TableMakerManager
{
   public static TableMaker AddToTable(TableMaker table, params object[] items)
   {
      table.Add(items);
      return table;
   }
}