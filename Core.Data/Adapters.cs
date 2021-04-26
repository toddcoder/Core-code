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
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data
{
   public class Adapters<T> : IEnumerable<Adapter<T>> where T : class
   {
      protected static StringHash<Func<DataGroups, string, ISetup>> setups;

      static Adapters()
      {
         setups = new StringHash<Func<DataGroups, string, ISetup>>(true)
         {
            ["sql"] = (dataGroups, adapterName) =>
            {
               var sqlSetup = SqlSetup.FromDataGroups(dataGroups, adapterName).ForceValue();
               sqlSetup.Handler = Handler;

               return sqlSetup;
            }
         };
      }

      public static IMaybe<SqlInfoMessageEventHandler> Handler { get; set; } = none<SqlInfoMessageEventHandler>();

      public static void RegisterSetup(string name, Func<DataGroups, string, ISetup> func) => setups[name] = func;

      public static IResult<Func<DataGroups, string, ISetup>> Setup(string setupType) => setups.Require(setupType);

      protected DataGroups dataGroups;
      protected StringHash<Adapter<T>> adapters;
      protected StringSet validAdapters;
      protected Predicate<string> isValidAdapterName;

      public Adapters(DataGroups dataGroups, params string[] validAdapterNames)
      {
         this.dataGroups = dataGroups;
         adapters = new StringHash<Adapter<T>>(true);
         validAdapters = new StringSet(true);
         if (validAdapterNames.Length == 0)
         {
            isValidAdapterName = _ => true;
         }
         else
         {
            isValidAdapterName = adapterName => adapterName.IsNotEmpty() && validAdapters.Contains(adapterName);
            validAdapters.AddRange(validAdapterNames);
         }
      }

      protected Adapters(DataGroups dataGroups, StringHash<Adapter<T>> adapters, StringSet validAdapters,
         Predicate<string> isValidAdapterName)
      {
         this.dataGroups = dataGroups;
         this.adapters = new StringHash<Adapter<T>>(true, adapters);
         this.validAdapters = new StringSet(true, validAdapters);
         this.isValidAdapterName = isValidAdapterName;
      }

      protected IResult<string> validAdapterName(string adapterName)
      {
         return isValidAdapterName(adapterName).Result(() => adapterName, $"Adapter name {adapterName} is invalid");
      }

      protected IResult<string> adapterExists(string adapterName)
      {
         return dataGroups.AdaptersGroup.RequireGroup(adapterName).Map(_ => adapterName);
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

      protected IResult<Adapter<T>> getAdapter(T entity, string child, Func<DataGroups, string, ISetup> setup) => tryTo(() =>
      {
         var adapter = adapters.Find(child, an => new Adapter<T>(entity, setup(dataGroups, an)), true);
         adapter.Entity = entity;

         return adapter.Success();
      });

      protected IResult<Adapter<T>> getAdapter(Func<T> alwaysUse, string child, Func<DataGroups, string, ISetup> setup)
      {
         return tryTo(() => adapters.Find(child, an => new Adapter<T>(alwaysUse(), setup(dataGroups, an)), true).Success());
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
         adapterName.Must().Not.BeNullOrEmpty().OrThrow();

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

      public Adapters<T> Clone() => new(dataGroups, adapters, validAdapters, isValidAdapterName);
   }

   public class Adapters : Adapters<DataContainer>
   {
      public Adapters(DataGroups dataGroups, params string[] validAdapterNames) : base(dataGroups, validAdapterNames)
      {
      }
   }
}