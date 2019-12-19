using System;
using System.IO;
using Core.Assertions;
using Core.ObjectGraphs.Parsers;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.ObjectGraphs
{
   public class ObjectGraphWriter : IDisposable
   {
      int indent;
      StringWriter writer;

      public ObjectGraphWriter()
      {
         indent = 0;
         writer = new StringWriter();
      }

      public void Begin() => indent++;

      public void End() => (--indent).MustAs(nameof(indent)).BeGreaterThan(-1).Assert("Too many unindents");

      public void Write(string name, string value = "", string type = "", bool quote = false)
      {
         writer.Write(indentation());
         writer.Write(name);
         if (type.IsNotEmpty())
         {
            writer.Write($": {type}");
         }

         if (value.IsNotEmpty())
         {
            if (value.IsMatch("[quote /r /n ';']") || quote)
            {
               writer.Write($" -> \"{value.Replace("\"", "`\"")}\"");
            }
            else
            {
               writer.Write($" -> {value}");
            }
         }

         writer.WriteLine();
      }

      public override string ToString() => writer.ToString();

      public ObjectGraph ToObjectGraph(string configs = Parser.DEFAULT_CONFIGS)
      {
         var parser = new Parser(writer.ToString());
         return parser.Parse(configs);
      }

      string indentation() => "\t".Repeat(indent);

      public void Dispose()
      {
         writer?.Dispose();
         GC.SuppressFinalize(this);
      }

      ~ObjectGraphWriter() => writer?.Dispose();
   }
}