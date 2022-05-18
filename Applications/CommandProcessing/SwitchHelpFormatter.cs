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
         if (source.Matches("'{' /(-['.']+) '...' /(-['}']+) '}'; f").Map(out var result))
         {
            for (var i = 0; i < result.MatchCount; i++)
            {
               var left = result[i, 1].Unjoin("/s* ',' /s*; f");
               var right = result[i, 2].Unjoin("/s* ',' /s*; f");
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
         string prefix, string shortCutPrefix, IHash<string, string> commandReplacements)
      {
         this.command = command;
         this.helpText = helpText;
         this.source = expand(source);

         replacements = new StringHash(true);

         foreach (var (name, (type, argument, _shortCut)) in switchHelp)
         {
            var builder = new StringBuilder($"{prefix}{name}");
            if (_shortCut.Map(out var shortCut))
            {
               builder.Append($" ({shortCutPrefix}{shortCut})");
            }

            if (!type.IsMatch("^ 'bool' ('ean')? $; f"))
            {
               builder.Append($" <{type}>");
            }

            var newArgument = commandReplacements.Map(name).DefaultTo(() => argument);
            builder.Append($" : {newArgument}");

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
            var _indent = Maybe<string>.nil;

            foreach (var line in source.Unjoin("/s* ';' /s*; f"))
            {
               if (_divider.Map(out var divider))
               {
                  writer.WriteLine(divider);
               }
               else
               {
                  _divider = "-".Repeat(length);
               }

               if (line.Matches(@"-(> '\') /('$' /w [/w '-']*) /('?')?; f").Map(out var result))
               {
                  for (var i = 0; i < result.MatchCount; i++)
                  {
                     var name = result[i, 1];
                     var optional = result[i, 2] == "?";
                     if (replacements.Map(name, out var replacement))
                     {
                        var indent = _indent.DefaultTo(() => " ");
                        var prefix = optional ? $"{indent}[" : indent;
                        var suffix = optional ? "]\r\n" : "\r\n";
                        result[i] = $"{prefix}{replacement}{suffix}";

                        if (_indent.IsNone)
                        {
                           _indent = " ".Repeat(command.Length + 1);
                        }
                     }
                     else
                     {
                        return fail($"Didn't understand '{result[i]}'");
                     }
                  }

                  writer.WriteLine($"{command}{result}");
                  _indent = nil;
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