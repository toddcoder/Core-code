﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Data.DataSources;
using Core.Monads;

namespace Core.Data
{
   public class Reader<T> : IDisposable, IEnumerable<T>
   {
      protected DataSource dataSource;
      protected Func<T> entityFunc;

      public Reader(DataSource dataSource, T entity, Func<T> entityFunc, string command, Parameters.Parameters parameters, Fields.Fields fields)
      {
         this.dataSource = dataSource;
         dataSource.BeginReading(entity, command, parameters, fields);
         Entity = entity;
         this.entityFunc = entityFunc;

         var outputParameters = new Parameters.Parameters();
         foreach (var dataParameter in dataSource.Command.Required("Command hasn't be set").Parameters.Cast<IDataParameter>()
            .Where(p => p.Direction == ParameterDirection.Output))
         {
            outputParameters[dataParameter.ParameterName] = parameters[dataParameter.ParameterName];
         }
      }

      public object Entity { get; set; }

      public IMaybe<T> Next() => dataSource.NextReading(entityFunc()).Map(obj => (T)obj);

      void IDisposable.Dispose()
      {
         dispose();
         GC.SuppressFinalize(this);
      }

      protected void dispose()
      {
         dataSource.EndReading();
      }

      ~Reader() => dispose();

      public IEnumerator<T> GetEnumerator()
      {
         try
         {
            while (Next().If(out var entity))
            {
               yield return entity;
            }
         }
         finally
         {
            dispose();
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}