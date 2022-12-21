using System.IO;
using System.Linq;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Loggers;

public class MultiLogger
{
   // ReSharper disable once CollectionNeverUpdated.Global
   protected AutoStringHash<Logger> loggers;
   protected StringSet keys;
   protected Maybe<string> _key;

   public MultiLogger(int indentation = 2)
   {
      loggers = new AutoStringHash<Logger>(true, key =>
      {
         keys.Add(key);
         return new Logger(indentation) { Key = key };
      });
      keys = new StringSet(true);
      _key = nil;
   }

   public Logger this[string key]
   {
      get
      {
         _key = key;
         return loggers[_key];
      }
   }

   public override string ToString()
   {
      using var writer = new StringWriter();
      var loggersMaybe = loggers.Maybe;
      foreach (var logger in keys.Select(key => loggersMaybe[key]).OnlyTrue())
      {
         var key = logger.Key | "";
         var minDateTime = logger.MinDateTime.Map(dt => $"{dt:O} ") | "";
         var maxDateTime = logger.MaxDateTime.Map(dt => $" {dt:O}") | "";

         writer.WriteLine($"[{key}--------{minDateTime}-{maxDateTime}]");
         logger.Flush(writer);
      }

      return writer.ToString();
   }
}