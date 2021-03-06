﻿using System;
using System.IO;
using Core.Assertions;
using Core.ObjectGraphs.Parsers;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Assertions.AssertionFunctions;

namespace Core.ObjectGraphs
{
   public class ObjectGraphWriter : IDisposable
   {
      protected int indent;
      protected StringWriter writer;

      public ObjectGraphWriter()
      {
         indent = 0;
         writer = new StringWriter();
      }

      public void Begin() => indent++;

      public void End()
      {
         indent--;
         assert(() => indent).Must().BeGreaterThan(-1).OrThrow("Too many unindents");
      }

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

      protected string indentation() => "\t".Repeat(indent);

      public void Dispose()
      {
         writer?.Dispose();
         GC.SuppressFinalize(this);
      }

      ~ObjectGraphWriter() => writer?.Dispose();
   }
}