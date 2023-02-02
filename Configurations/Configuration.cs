using System;
using System.Text;
using Core.Collections;
using Core.Computers;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Configurations;

public class Configuration : Setting
{
   public static Result<Configuration> Open(FileName file)
   {
      return
         from source in file.TryTo.Text
         from setting in FromString(source)
         select new Configuration(file, setting.items);
   }

   public static Result<Configuration> Serialize(FileName file, Type type, object obj, bool save = true, string name = ROOT_NAME)
   {
      var _setting = Serialize(type, obj, name);
      if (_setting is (true, var setting))
      {
         var configuration = new Configuration(file, setting.items, name);
         if (save)
         {
            return configuration.Save();
         }
         else
         {
            return configuration;
         }
      }
      else
      {
         return _setting.Exception;
      }
   }

   public static Result<Configuration> Serialize<T>(FileName file, T obj, bool save = true, string name = ROOT_NAME) where T : class, new()
   {
      return tryTo(() => Serialize(file, typeof(T), obj, save, name));
   }

   protected FileName file;

   internal Configuration(FileName file, StringHash<ConfigurationItem> items, string name = ROOT_NAME) : base(name)
   {
      this.file = file;
      this.items = items;
   }

   public FileName File => file;

   public Result<Configuration> Save() => file.TryTo.SetText(ToString(), Encoding.UTF8).Map(_ => this);
}