using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Data.Configurations;
using Core.Data.Setups;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data
{
   public class Adapters<T> : IEnumerable<Adapter<T>>
      where T : class
   {
      static Hash<string, Func<DataGraphs, string, ISetup>> setups;

      static Adapters()
      {
         setups = new Hash<string, Func<DataGraphs, string, ISetup>>
         {
            ["sql"] = (dg, an) => new SQLSetup(dg, an) { Handler = Handler }
         };
      }

      public static IMaybe<SqlInfoMessageEventHandler> Handler { get; set; } = none<SqlInfoMessageEventHandler>();

      public static void RegisterSetup(string name, Func<DataGraphs, string, ISetup> func) => setups[name] = func;

      public static IResult<Func<DataGraphs, string, ISetup>> Setup(string setupType) => setups.Require(setupType);

      DataGraphs dataGraphs;
      Hash<string, Adapter<T>> adapters;
      Set<string> validAdapters;
      Predicate<string> isValidAdapterName;

      public Adapters(DataGraphs dataGraphs, params string[] validAdapterNames)
      {
         this.dataGraphs = dataGraphs;
         adapters = new Hash<string, Adapter<T>>();
         validAdapters = new Set<string>();
         if (validAdapterNames.Length == 0)
         {
            isValidAdapterName = adapterName => true;
         }
         else
         {
            isValidAdapterName = adapterName => adapterName.IsNotEmpty() && validAdapters.Contains(adapterName);
            validAdapters.AddRange(validAdapterNames);
         }
      }

      protected Adapters(DataGraphs dataGraphs, Hash<string, Adapter<T>> adapters, Set<string> validAdapters,
         Predicate<string> isValidAdapterName)
      {
         this.dataGraphs = dataGraphs;
         this.adapters = new Hash<string, Adapter<T>>(adapters);
         this.validAdapters = new Set<string>(validAdapters);
         this.isValidAdapterName = isValidAdapterName;
      }

      IResult<string> validAdapterName(string adapterName)
      {
         return isValidAdapterName(adapterName).Result(() => adapterName, $"Adapter name {adapterName} is invalid");
      }

      IResult<string> adapterExists(string adapterName)
      {
         return dataGraphs.AdaptersGraph.ChildExists(adapterName)
            .Result(() => adapterName, $"Adapter name {adapterName} doesn't exist");
      }

      public IResult<Adapter<T>> Adapter(string adapterName, T entity, string setupType = "sql")
      {
         if (adapters.ContainsKey(adapterName))
         {
            var adapter = adapters[adapterName];
            adapter.Entity = entity;

            return adapter.Success();
         }

         return
            from name in validAdapterName(adapterName)
            from childName in adapterExists(name)
            from setup in Setup(setupType)
            from adapter in getAdapter(entity, childName, setup)
            select adapter;
      }

      IResult<Adapter<T>> getAdapter(T entity, string child, Func<DataGraphs, string, ISetup> setup) => tryTo(() =>
      {
         var adapter = adapters.Find(child, an => new Adapter<T>(entity, setup(dataGraphs, an)), true);
         adapter.Entity = entity;

         return adapter.Success();
      });

      IResult<Adapter<T>> getAdapter(Func<T> alwaysUse, string child, Func<DataGraphs, string, ISetup> setup)
      {
         return tryTo(() => adapters.Find(child, an => new Adapter<T>(alwaysUse(), setup(dataGraphs, an)), true).Success());
      }

      public IResult<TResult> Execute<TResult>(string adapterName, T entity, Func<T, TResult> map, string setupType = "sql")
      {
         if (adapters.ContainsKey(adapterName))
         {
            var adapter = adapters[adapterName];
            adapter.Entity = entity;

            return adapter.TryTo.Execute().Map(map);
         }

         return
            from name in validAdapterName(adapterName)
            from childName in adapterExists(name)
            from adapter in Adapter(childName, entity, setupType)
            from obj in adapter.TryTo.Execute()
            from result in map(obj).Success()
            select result;
      }

      public IResult<T> Execute(string adapterName, T entity, string setupType = "sql")
      {
         if (adapters.ContainsKey(adapterName))
         {
            var adapter = adapters[adapterName];
            adapter.Entity = entity;

            return adapter.TryTo.Execute();
         }

         return
            from name in validAdapterName(adapterName)
            from childName in adapterExists(name)
            from adapter in Adapter(childName, entity, setupType)
            from obj in adapter.TryTo.Execute()
            select obj;
      }

      public IResult<Adapter<T>> Adapter(string adapterName, Func<T> entityFunc, string setupType = "sql")
      {
         if (adapters.ContainsKey(adapterName))
         {
            var adapter = adapters[adapterName];
            adapter.Entity = entityFunc();

            return adapter.Success();
         }

         return
            from name in validAdapterName(adapterName)
            from childName in adapterExists(name)
            from setup in Setup(setupType)
            from adapter in getAdapter(entityFunc(), childName, setup)
            select adapter;
      }

      public IResult<T> Execute(string adapterName, Func<T> entityFunc, string setupType = "sql")
      {
         if (adapters.ContainsKey(adapterName))
         {
            var adapter = adapters[adapterName];
            adapter.Entity = entityFunc();

            return adapter.TryTo.Execute();
         }

         return
            from name in validAdapterName(adapterName)
            from childName in adapterExists(name)
            from adapter in Adapter(childName, entityFunc(), setupType)
            from obj in adapter.TryTo.Execute()
            select obj;
      }

      public IEnumerator<Adapter<T>> GetEnumerator() => adapters.Values.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IEnumerable<Adapter<T>> AllAdapters(Func<string, T> func, string setupType = "sql")
      {
         return validAdapters.Select(name => Adapter(name, func(name), setupType)).SuccessfulValue();
      }

      public void Add(string adapterName)
      {
         assert(() => adapterName).Must().Not.BeNullOrEmpty().OrThrow();

         validAdapters.Add(adapterName);
      }

      public IResult<IBulkCopyTarget> BulkCopy(string sourceAdapterName, string targetAdapterName, Func<T> entityFunc,
         string sourceSetupType = "sql")
      {
         return
            from sourceAdapter in Adapter(sourceAdapterName, entityFunc, sourceSetupType)
            from targetAdapter in Adapter(targetAdapterName, entityFunc)
            from target in targetAdapter.TryTo.BulkCopy(sourceAdapter)
            select target;
      }

      public IResult<IBulkCopyTarget> BulkCopy(string sourceAdapterName, string targetAdapterName, T entity,
         string sourceSetupType = "sql")
      {
         return
            from sourceAdapter in Adapter(sourceAdapterName, entity, sourceSetupType)
            from targetAdapter in Adapter(targetAdapterName, entity)
            from target in targetAdapter.TryTo.BulkCopy(sourceAdapter)
            select target;
      }

      public IEnumerable<IResult<T>> ExecuteAll(T entity, string setupType = "sql")
      {
         return validAdapters.Select(key => Execute(key, entity, setupType));
      }

      public IEnumerable<IResult<T>> ExecuteAll(Func<string, T> map, string setupType = "sql")
      {
         return validAdapters
            .Select(name => new { Name = name, Entity = tryTo(() => map(name)) })
            .Select(result => result.Entity.Map(entity => Execute(result.Name, entity, setupType)));
      }

      public string[] Names => adapters.KeyArray();

      public Adapters<T> Clone() => new Adapters<T>(dataGraphs, adapters, validAdapters, isValidAdapterName);
   }

   public class Adapters : Adapters<DataContainer>
   {
      public Adapters(DataGraphs dataGraphs, params string[] validAdapterNames)
         : base(dataGraphs, validAdapterNames) { }
   }
}