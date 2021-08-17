using System;
using System.IO;
using System.Linq;
using Core.Assertions;

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
         nameSpace = $"{type.Namespace}.";
         names = type.Assembly.GetManifestResourceNames();
      }

      public Resources(string path)
      {
         type = typeof(T);
         nameSpace = $@"{type.Namespace}.{path}.";
         names = type.Assembly.GetManifestResourceNames();
      }

      public string String(string name)
      {
         Contains(name).Must().BeTrue().OrThrow($"Resource {nameSpace}{name} does not exist");
         using var reader = new StreamReader(Stream(name));

         return reader.ReadToEnd();
      }

      public Stream Stream(string name)
      {
         var fullName = $"{nameSpace}{name}";
         var message = $"Resource {fullName} does not exist";
         Contains(name).Must().BeTrue().OrThrow(message);

         var stream = type.Assembly.GetManifestResourceStream(fullName);
         stream.Must().Not.BeNull().OrThrow(message);

         return stream;
      }

      public byte[] Bytes(string name)
      {
         Contains(name).Must().BeTrue().OrThrow($"Resource {nameSpace}{name} does not exist");
         using var stream = Stream(name);

         var length = (int)stream.Length;
         var buffer = new byte[length];

         while (length > 0)
         {
            var numberOfBytesRead = stream.Read(buffer, 0, length);

            if (numberOfBytesRead == 0)
            {
               break;
            }

            length -= numberOfBytesRead;
         }

         return buffer;
      }

      public bool Contains(string name) => names.Contains(nameSpace + name);
   }
}