using System;
using Core.Collections;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.ObjectGraphs;

namespace Core.Data
{
   public class Connection : IHash<string, string>
   {
      protected Hash<string, string> data;

      public Connection(ObjectGraph connectionGraph)
      {
         data = new Hash<string, string>();
         Name = connectionGraph.Name;
         Type = connectionGraph.FlatMap("type", g => g.Value, "sql");
         Timeout = connectionGraph.FlatMap("timeout", g => g.Value.ToTimeSpan(), () => 30.Seconds());
         foreach (var child in connectionGraph.Children)
         {
            data[child.Name] = child.Value;
         }
      }

      public string Name { get; set; }

      public string this[string name]
      {
         get => data[name];
         set => data[name] = value;
      }

      public bool ContainsKey(string key) => data.ContainsKey(key);

      public IResult<Hash<string, string>> AnyHash() => data.Success();

      public string Type { get; set; }

      public TimeSpan Timeout { get; set; }
   }
}