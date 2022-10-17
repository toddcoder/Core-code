using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.DataStructures;

public class EnumerableQueue<T>
{
   protected IEnumerator<T> enumerator;

   public EnumerableQueue(IEnumerable<T> enumerable)
   {
      enumerator = enumerable.GetEnumerator();
   }

   public Responding<T> Next()
   {
      try
      {
         if (enumerator.MoveNext())
         {
            try
            {
               return enumerator.Current;
            }
            catch (Exception innerException)
            {
               return innerException;
            }
         }
         else
         {
            return nil;
         }
      }
      catch (Exception outerException)
      {
         return outerException;
      }
   }
}