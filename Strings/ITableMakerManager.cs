namespace Core.Strings;

public interface ITableMakerManager
{
   TableMaker Add(TableMaker table, params object[] items);
}