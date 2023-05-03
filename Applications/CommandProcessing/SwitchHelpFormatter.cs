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
using static Core.Monads.Monads;

namespace Core.Applications.CommandProcessing;

public class SwitchHelpFormatter
{
   protected static string expand(string source)
   {
      var _matches = source.Matches("'{' /(-['.']+) '...' /(-['}']+) '}'; f");
      if (_matches is (true, var matches))
      {
         foreach (var match in matches)
         {
            var left = match.FirstGroup.Unjoin("/s* ',' /s*; f");
            var right = match.SecondGroup.Unjoin("/s* ',' /s*; f");
            var list = new List<string>();
            foreach (var leftItem in left)
            {
               foreach (var rightItem in right)
               {
                  list.Add($"{leftItem}{rightItem}");
               }
            }

            match.Text = list.ToString(";");
         }

         return matches.ToString();
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
         if (_shortCut is (true, var shortCut))
         {
            builder.Append($" ({shortCutPrefix}{shortCut})");
         }

         if (!type.IsMatch("^ 'bool' ('ean')? $; f"))
         {
            builder.Append($" <{type}>");
         }

         var newArgument = commandReplacements.Items[name] | argument;
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

         var _divider = monads.maybe<string>();
         var _indent = monads.maybe<string>();

         foreach (var line in source.Unjoin("/s* ';' /s*; f"))
         {
            if (_divider is (true, var divider))
            {
               writer.WriteLine(divider);
            }
            else
            {
               _divider = "-".Repeat(length);
            }

            var _matches = line.Matches(@"-(> '\') /('$' /w [/w '-']*) /('?')?; f");
            if (_matches is (true, var matches))
            {
               foreach (var match in matches)
               {
                  var name = match.FirstGroup;
                  var optional = match.SecondGroup == "?";
                  var _replacement = replacements.Maybe[name];
                  if (_replacement is (true, var replacement))
                  {
                     var indent = _indent | " ";
                     var prefix = optional ? $"{indent}[" : indent;
                     var suffix = optional ? "]\r\n" : "\r\n";
                     match.Text = $"{prefix}{replacement}{suffix}";

                     if (!_indent)
                     {
                        _indent = " ".Repeat(command.Length + 1);
                     }
                  }
                  else
                  {
                     return fail($"Didn't understand '{match}'");
                  }
               }

               writer.WriteLine($"{command}{_matches}");
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