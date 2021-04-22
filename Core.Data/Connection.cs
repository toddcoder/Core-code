﻿using System;
using Core.Collections;
using Core.Configurations;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Monads;

namespace Core.Data
{
   public class Connection : IHash<string, string>
   {
      protected StringHash<string> data;

      public Connection(Group connectionGroup)
      {
         data = new StringHash<string>(true);
         Name = connectionGroup.Key;
         Type = connectionGroup.GetValue("type").DefaultTo(() => "sql");
         Timeout = connectionGroup.GetValue("timeout").Map(s => s.ToTimeSpan()).DefaultTo(() => 30.Seconds());
         foreach (var (key, value) in connectionGroup.Values())
         {
            data[key] = value;
         }
      }

      public string Name { get; set; }

      public string this[string name]
      {
         get => data[name];
         set => data[name] = value;
      }

      public bool ContainsKey(string key) => data.ContainsKey(key);

      public IResult<Hash<string, string>> AnyHash() => data.AsHash.Success();

      public string Type { get; set; }

      public TimeSpan Timeout { get; set; }
   }
}