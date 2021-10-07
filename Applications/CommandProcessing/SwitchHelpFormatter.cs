using System;
using System.IO;
using System.Text;
using Core.Collections;
using Core.Monads;
using Core.Matching;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.CommandProcessing
{
   public class SwitchHelpFormatter
   {
      protected string command;
      protected string source;
      protected StringHash replacements;

      public SwitchHelpFormatter(string command, string source, StringHash<(string, Maybe<string>, Maybe<string>)> switchHelp, string prefix,
         string shortCutPrefix)
      {
         this.command = command;
         this.source = source;

         replacements = new StringHash(true);

         foreach (var (name, (type, _argument, _shortCut)) in switchHelp)
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

            if (_argument.If(out var argument))
            {
               builder.Append($" : {argument}");
            }

            replacements[$"${name}"] = builder.ToString();
         }
      }

      public Result<string> Format()
      {
         try
         {
            using var writer = new StringWriter();

            writer.WriteLine(command);
            var commandLength = command.Length.MaxOf(80);
            writer.WriteLine("=".Repeat(commandLength));

            var _divider = MaybeOf<string>.nil;

            foreach (var line in source.Split("/s* ';' /s*; f"))
            {
               if (_divider.If(out var divider))
               {
                  writer.WriteLine(divider);
               }
               else
               {
                  _divider = "-".Repeat(commandLength);
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

                  writer.WriteLine(result.ToString());
               }
               else
               {
                  writer.WriteLine(line.Replace(@"\$", "$"));
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