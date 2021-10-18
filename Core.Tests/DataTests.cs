using System;
using System.Collections.Generic;
using System.Linq;
using Core.Applications;
using Core.Collections;
using Core.Configurations;
using Core.Data;
using Core.Data.ConnectionStrings;
using Core.Data.Fields;
using Core.Data.Parameters;
using Core.Data.Setups;
using Core.Dates.DateIncrements;
using Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   internal class ColumnData : ISetupObject
   {
      public ColumnData()
      {
         TypeName = string.Empty;
         Name = string.Empty;
      }

      public string Name { get; set; }

      public string TypeName { get; set; }

      public int ObjectId { get; set; }

      public override string ToString() => $"{ObjectId} = {Name} {TypeName}";

      public string ConnectionString => SqlConnectionString.GetConnectionString(".", "local_tebennett", "TSqlCop");

      public CommandSourceType CommandSourceType => CommandSourceType.File;

      public string Command => @"C:\Enterprise\Projects\TSqlCop\source\SqlConformance.Library\MetaData\Queries\Columns.sql";

      public TimeSpan CommandTimeout => 30.Seconds();

      public IEnumerable<Parameter> Parameters()
      {
         yield return new Parameter("@lObjectId", nameof(ObjectId), typeof(int));
      }

      public IEnumerable<Field> Fields()
      {
         yield return new Field(nameof(ObjectId), typeof(int));
         yield return new Field(nameof(Name), typeof(string));
         yield return new Field(nameof(TypeName), typeof(string));
      }

      public IHash<string, string> Attributes => new Hash<string, string>();

      public ISetup Setup() => new SqlSetup(this);
   }

   [TestClass]
   public class DataTests
   {
      protected const string TRUE_CONNECTION_STRING = "Data Source=.;Initial Catalog=local_tebennett;Integrated Security=SSPI;" +
         "Application Name=TSqlCop;";

      [TestMethod]
      public void FromConfigurationTest()
      {
         var entity = new ColumnData { ObjectId = 95 };
         var resources = new Resources<DataTests>();
         var source = resources.String("TestData.data.configuration");
         var _adapter =
            from @group in Group.FromString(source)
            from setup in SqlSetup.FromGroup(@group, "all")
            from adapter in Adapter<ColumnData>.FromSetup(setup, entity)
            select adapter;
         if (_adapter.If(out var allColumnData, out var exception))
         {
            var data = allColumnData.ToArray();
            foreach (var columnData in data)
            {
               Console.WriteLine(columnData);
            }
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void FromConnectionStringTest()
      {
         var resources = new Resources<DataTests>();
         var source = resources.String("TestData.data.configuration");
         var _adapter =
            from configuration in Group.FromString(source)
            from setup in SqlSetup.FromGroup(configuration, "all2")
            from adapter in Adapter<ColumnData>.FromSetup(setup, new ColumnData { ObjectId = 5664280 })
            select adapter;
         if (_adapter.If(out var allColumnData, out var exception))
         {
            allColumnData.ConnectionString = TRUE_CONNECTION_STRING;
            foreach (var columnData in allColumnData)
            {
               Console.WriteLine(columnData);
            }
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void FromSetupObject()
      {
         var entity = new ColumnData { ObjectId = 89 };
         var _adapter = Adapter<ColumnData>.FromSetupObject(entity);
         if (_adapter.If(out var allColumnData, out var exception))
         {
            allColumnData.ConnectionString = TRUE_CONNECTION_STRING;
            var data = allColumnData.ToArray();
            foreach (var columnData in data)
            {
               Console.WriteLine(columnData);
            }
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void SignatureTest()
      {
         var signature = new Signature("Foobar");
         Console.WriteLine(signature);

         signature = new Signature("Foobar[153]");
         Console.WriteLine(signature);
      }

      protected class Object : ISetupObject
      {
         public Object()
         {
            ObjectName = string.Empty;
         }

         public string ObjectName { get; set; }

         public int ObjectId { get; set; }

         public string ConnectionString => SqlConnectionString.GetConnectionString(".", "local_tebennett", "TSqlCop");

         public CommandSourceType CommandSourceType => CommandSourceType.SQL;

         public string Command => "SELECT name as ObjectName, object_id as ObjectId FROM sys.objects WHERE name = @lObjectName";

         public TimeSpan CommandTimeout => 30.Seconds();

         public IEnumerable<Parameter> Parameters()
         {
            yield return new Parameter("@lObjectName", nameof(ObjectName), typeof(string));
         }

         public IEnumerable<Field> Fields()
         {
            yield return new Field(nameof(ObjectName), typeof(string));
            yield return new Field(nameof(ObjectId), typeof(int));
         }

         public IHash<string, string> Attributes => new Hash<string, string>();

         public ISetup Setup() => new SqlSetup(this);
      }

      [TestMethod]
      public void HasRowsTest()
      {
         var obj = new Object { ObjectName = "Foobar" };
         Console.WriteLine(obj.SqlAdapter().ExecuteMaybe().IsSome ? "Foobar exists" : "Foobar doesn't exist");

         obj.ObjectName = "PaperTicketStorageAssignment";
         Console.WriteLine(obj.SqlAdapter().ExecuteMaybe().IsSome ? $"{obj.ObjectName} exists" : $"{obj.ObjectName} doesn't exist");
      }
   }
}