using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Matching;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing
{
   public class SwitchHelpFormatter
   {
      protected static string expand(string source)
      {
         if (source.Matches("'{' /(-['.']+) '...' /(-['}']+) '}'; f").If(out var result))
         {
            for (var i = 0; i < result.MatchCount; i++)
            {
               var left = result[i, 1].Split("/s* ',' /s*; f");
               var right = result[i, 2].Split("/s* ',' /s*; f");
               var list = new List<string>();
               foreach (var leftItem in left)
               {
                  foreach (var rightItem in right)
                  {
                     list.Add($"{leftItem}{rightItem}");
                  }
               }

               result[i] = list.ToString(";");
            }

            return result.ToString();
         }
         else
         {
            return source;
         }
      }

      protected string command;
      protected string helpText;
      protected string source;
      protected StringHash replacements;

      public SwitchHelpFormatter(string command, string helpText, string source, StringHash<(string, string, Maybe<string>)> switchHelp,
         string prefix, string shortCutPrefix)
      {
         this.command = command;
         this.helpText = helpText;
         this.source = expand(source);

         replacements = new StringHash(true);

         foreach (var (name, (type, argument, _shortCut)) in switchHelp)
         {
            var builder = new StringBuilder($"{prefix}{name}");
            if (_shortCut.If(out var shortCut))
            {
               builder.Append($" ({shortCutPrefix}{shortCut})");
            }

            if (!type.IsMatch("^ 'bool' ('ean')? $; f"))
            {
               builder.Append($" <{type}>");
            }

            builder.Append($" : {argument}");

            replacements[$"${name}"] = builder.ToString();
         }
      }

      public Result<string> Format()
      {
         try
         {
            using var writer = new StringWriter();

            var firstLine = $"{command} - {helpText}";
            writer.WriteLine(firstLine);
            var length = firstLine.Length.MaxOf(80);
            writer.WriteLine("=".Repeat(length));

            var _divider = Maybe<string>.nil;

            foreach (var line in source.Split("/s* ';' /s*; f"))
            {
               if (_divider.If(out var divider))
               {
                  writer.WriteLine(divider);
               }
               else
               {
                  _divider = "-".Repeat(length);
               }

               if (line.Matches(@"-(> '\') /('$' /w [/w '-']*) /('?')?; f").If(out var result))
               {
                  for (var i = 0; i < result.MatchCount; i++)
                  {
                     var name = result[i, 1];
                     var optional = result[i, 2] == "?";
                     if (replacements.If(name, out var replacement))
                     {
                        var prefix = optional ? "\t[" : "\t";
                        var suffix = optional ? "]\r\n" : "\r\n";
                        result[i] = $"{prefix}{replacement}{suffix}";
                     }
                     else
                     {
                        return fail($"Didn't understand '{result[i]}'");
                     }
                  }

                  writer.WriteLine($"{command}{result}");
               }
               else
               {
                  writer.WriteLine($"{command} {line.Replace(@"\$", "$")}");
               }
            }

            return writer.ToString();
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }
}