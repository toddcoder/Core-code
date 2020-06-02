using System;
using Core.Data;
using Core.Data.Setups;
using Core.ObjectGraphs.Configurations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   internal class ColumnData
   {
      public string ObjectName { get; set; } = "";

      public string ColumnName { get; set; } = "";

      public string TypeName { get; set; } = "";

      public override string ToString() => $"{ObjectName}.{ColumnName} {TypeName}";
   }

   [TestClass]
   public class DataTests
   {
      const string TRUE_CONNECTION_STRING = "Data Source=.;Initial Catalog=local_tebennett;Integrated Security=SSPI;";

      [TestMethod]
      public void FromConfigurationTest()
      {
         var anyAdapter =
            from configuration in Configuration.LoadFromObjectGraph(@"C:\Enterprise\Projects\Core\Core.Tests\test-data\configuration.objectgraph")
            from setup in SQLSetup.FromConfiguration(configuration, "all")
            from adapter in Adapter<ColumnData>.FromSetup(setup, new ColumnData())
            select adapter;
         if (anyAdapter.If(out var allColumnData, out var exception))
         {
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
   }
}