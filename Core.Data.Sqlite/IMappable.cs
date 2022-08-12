using System.Data.SQLite;
using Core.Collections;

namespace Core.Data.Sqlite;

public interface IMappable
{
   StringHash<object> FromObject();

   void ToObject(SQLiteDataReader reader);

   void ToObject(StringHash fieldValues);
}