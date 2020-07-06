using System;
using System.IO;
using System.Linq;
using Core.Assertions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Applications
{
   public class Resources<T>
   {
      protected Type type;
      protected string nameSpace;
      protected string[] names;

      public Resources()
      {
         type = typeof(T);
         nameSpace = type.Namespace + ".";
         names = type.Assembly.GetManifestResourceNames();
      }

      public string String(string name)
      {
         assert(() => Contains(name)).Must().BeTrue().OrThrow($"Resource {nameSpace + name} does not exist");
         using (var reader = new StreamReader(Stream(name)))
         {
            return reader.ReadToEnd();
         }
      }

      public Stream Stream(string name)
      {
         var fullName = nameSpace + name;
         var message = $"Resource {fullName} does not exist";
         assert(() => Contains(name)).Must().BeTrue().OrThrow(message);

         var stream = type.Assembly.GetManifestResourceStream(fullName);
         assert(() => (object)stream).Must().Not.BeNull().OrThrow(message);

         return stream;
      }

      public bool Contains(string name) => names.Contains(nameSpace + name);
   }
}