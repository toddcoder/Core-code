using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Data;
using Core.Data.ConnectionStrings;
using Core.Data.Fields;
using Core.Data.Parameters;
using Core.Data.Setups;
using Core.Dates.DateIncrements;
using Core.ObjectGraphs.Configurations;
using Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   internal class ColumnData : ISetupObject
   {
      public string SchemaName { get; set; } = "";

      public string ObjectName { get; set; } = "";

      public string ColumnName { get; set; } = "";

      public string TypeName { get; set; } = "";

      public override string ToString() => $"{ObjectName}.{ColumnName} {TypeName}";

      public string ConnectionString => SQLConnectionString.GetConnectionString(".", "local_tebennett");

      public CommandSourceType CommandSourceType => CommandSourceType.File;

      public string Command => @"C:\Enterprise\Projects\TSqlCop\TSqlCop.Ssms\Queries\ColumnNames.sql";

      public TimeSpan CommandTimeout => 30.Seconds();

      public IEnumerable<Parameter> Parameters()
      {
         yield return new Parameter("@lSchemaName", nameof(SchemaName), typeof(string));
      }

      public IEnumerable<Field> Fields()
      {
         yield return new Field(nameof(SchemaName), typeof(string));
         yield return new Field(nameof(ObjectName), typeof(string));
         yield return new Field(nameof(ColumnName), typeof(string));
         yield return new Field(nameof(TypeName), typeof(string));
      }

      public IHash<string, string> Attributes => new Hash<string, string>();

      public ISetup Setup() => new SQLSetup(this);
   }

   [TestClass]
   public class DataTests
   {
      const string TRUE_CONNECTION_STRING = "Data Source=.;Initial Catalog=local_tebennett;Integrated Security=SSPI;";

      [TestMethod]
      public void FromConfigurationTest()
      {
         var entity = new ColumnData { SchemaName = "PreFlow" };
         var anyAdapter =
            from configuration in Configuration.LoadFromObjectGraph(@"C:\Enterprise\Projects\Core\Core.Tests\test-data\configuration.objectgraph")
            from setup in SQLSetup.FromConfiguration(configuration, "all")
            from adapter in Adapter<ColumnData>.FromSetup(setup, entity)
            select adapter;
         if (anyAdapter.If(out var allColumnData, out var exception))
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
         var anyAdapter =
            from configuration in Configuration.LoadFromObjectGraph(@"C:\Enterprise\Projects\Core\Core.Tests\test-data\configuration.objectgraph")
            from setup in SQLSetup.FromConfiguration(configuration, "all2")
            from adapter in Adapter<ColumnData>.FromSetup(setup, new ColumnData())
            select adapter;
         if (anyAdapter.If(out var allColumnData, out var exception))
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
         var entity = new ColumnData { SchemaName = "PreFlow" };
         var anyAdapter = Adapter<ColumnData>.FromSetupObject(entity);
         if (anyAdapter.If(out var allColumnData, out var exception))
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
   }
}