﻿using System.Collections.Generic;
using System.Text;
using Core.DataStructures;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Settings
{
   internal class Parser
   {
      protected const string REGEX_KEY = "/(['$@']? [/w '?'] [/w '-']*)";

      protected string source;

      public Parser(string source)
      {
         this.source = source;
      }

      public Result<Setting> Parse()
      {
         var root = new SettingBuilder(Setting.ROOT_KEY);
         var stack = new MaybeStack<SettingBuilder>();
         stack.Push(root);

         while (source.Length > 0)
         {
            if (source.Matches("^ /s* '['; f").Map(out var result))
            {
               var key = GetKey("?");
               var builder = new SettingBuilder(key);
               if (stack.Peek().Map(out var parentBuilder))
               {
                  parentBuilder.SetSubSetting(key, builder);
               }
               else
               {
                  return fail("No parent setting found");
               }

               stack.Push(builder);
               source = source.Drop(result.Length);
            }
            else if (source.Matches($"^ /s* {REGEX_KEY} /s* '['; f").Map(out result))
            {
               var key = GetKey(result.FirstGroup);
               var builder = new SettingBuilder(key);
               if (stack.Peek().Map(out var parentBuilder))
               {
                  parentBuilder.SetSubSetting(key, builder);
               }
               else
               {
                  return fail("No parent setting found");
               }

               stack.Push(builder);
               source = source.Drop(result.Length);
            }
            else if (source.Matches("^ /s* ']'; f").Map(out result))
            {
               if (stack.Pop().Map(out var builder))
               {
                  if (stack.Peek().Map(out var parentBuilder))
                  {
                     parentBuilder.SetSubSetting(builder.Key, builder);
                  }
                  else
                  {
                     return fail("No parent setting found");
                  }
               }
               else
               {
                  return fail("Not closing on setting");
               }

               source = source.Drop(result.Length);
            }
            else if (source.Matches($"^ /s* {REGEX_KEY} '.'; f").Map(out result))
            {
               var key = GetKey(result.FirstGroup);
               var builder = new SettingBuilder(key);
               if (stack.Peek().Map(out var parentGroup))
               {
                  parentGroup.SetSubSetting(key, builder);
               }
               else
               {
                  return fail("No parent group found");
               }

               source = source.Drop(result.Length);
            }
            else if (source.Matches("^ /s* '#' -[/r /n]*; f").Map(out result))
            {
               source = source.Drop(result.Length);
            }
            else if (source.Matches("^ /s* ':' /s*; f").Map(out result))
            {
               var key = GenerateKey();
               var remainder = source.Drop(result.Length);
               if (getString(source.Drop(result.Length)).Map(out source, out var value, out var isArray))
               {
                  var builder = new SettingBuilder(key);
                  builder.SetText(value);
                  builder.IsArray = isArray;
                  if (stack.Peek().Map(out var parentBuilder))
                  {
                     parentBuilder.SetSubSetting(key, builder);
                  }
               }
               else if (source.IsMatch("^ /s+ $; f"))
               {
                  break;
               }
               else
               {
                  return fail($"Didn't understand value {remainder}");
               }
            }
            else if (source.Matches($"^ /s* {REGEX_KEY} ':' /s*; f").Map(out result))
            {
               var key = GetKey(result.FirstGroup);
               var remainder = source.Drop(result.Length);
               if (getString(source.Drop(result.Length)).Map(out source, out var value, out var isArray))
               {
                  var builder = new SettingBuilder(key);
                  builder.SetText(value);
                  builder.IsArray = isArray;
                  if (stack.Peek().Map(out var parentBuilder))
                  {
                     parentBuilder.SetSubSetting(key, builder);
                  }
               }
               else if (source.IsMatch("^ /s+ $; f"))
               {
                  break;
               }
               else
               {
                  return fail($"Didn't understand value {remainder}");
               }
            }
            else if (source.IsMatch("^ /s+ $; f"))
            {
               break;
            }
            else if (getString(source.TrimLeft()).Map(out source, out var value, out var isArray))
            {
               var key = GenerateKey();
               var builder = new SettingBuilder(key);
               builder.SetText(value);
               builder.IsArray = isArray;
               if (stack.Peek().Map(out var parentBuilder))
               {
                  parentBuilder.SetSubSetting(key, builder);
               }
            }
            else
            {
               return fail($"Didn't understand {source.KeepUntil("\r\n")}");
            }
         }

         while (stack.Pop().Map(out var builder))
         {
            if (stack.Peek().Map(out var parentBuilder))
            {
               parentBuilder.SetSubSetting(builder.Key, builder);
            }
            else
            {
               break;
            }
         }

         return root.Setting();
      }

      protected Result<(string, string, bool)> getLinesAsArray(string source)
      {
         var lines = new List<string>();
         while (source.Length > 0)
         {
            if (source.Matches("^ /s* '}' ([/r /n]+ | $); f").Map(out var result))
            {
               return (source.Drop(result.Length), lines.ToString(","), true);
            }
            else if (source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f").Map(out result))
            {
               if (getString(result.FirstGroup).Map(out _, out var @string, out _, out var exception))
               {
                  source = source.Drop(result.Length);
                  lines.Add(@string);
               }
               else
               {
                  return exception;
               }
            }
         }

         return fail("No terminating }");
      }

      protected Result<(string newSource, string str, bool isArray)> getString(string source)
      {
         if (source.Matches("^ /s* /[quote]; f").Map(out var result))
         {
            var quote = result.FirstGroup[0];
            return getQuotedString(source.Drop(result.Length), quote);
         }
         else if (source.Matches("^ /s* '{' [/r /n]+; f").Map(out result))
         {
            var newSource = source.Drop(result.Length);
            return getLinesAsArray(newSource);
         }
         else if (source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f").Map(out result))
         {
            var foundReturn = false;
            var builder = new StringBuilder();
            for (var i = 0; i < source.Length; i++)
            {
               var current = source[i];
               switch (current)
               {
                  case ';':
                     return (source.Drop(i + 1), builder.ToString(), false);
                  case ']' or '#':
                     return (source.Drop(i), builder.ToString(), false);
                  case '\r' or '\n':
                     foundReturn = true;
                     break;
                  default:
                     if (foundReturn)
                     {
                        return (source.Drop(i - 1), builder.ToString(), false);
                     }
                     else
                     {
                        builder.Append(current);
                     }

                     break;
               }
            }

            return (string.Empty, builder.ToString(), false);
         }
         else
         {
            return fail("Couldn't determine string");
         }
      }

      protected static Result<(string newSource, string str, bool isArray)> getQuotedString(string source, char quote)
      {
         var escaped = false;
         var builder = new StringBuilder();

         for (var i = 0; i < source.Length; i++)
         {
            var current = source[i];
            switch (current)
            {
               case '`':
                  if (escaped)
                  {
                     builder.Append("`");
                     escaped = false;
                  }
                  else
                  {
                     escaped = true;
                  }

                  break;
               case 't':
                  if (escaped)
                  {
                     builder.Append("\t");
                     escaped = false;
                  }
                  else
                  {
                     builder.Append('t');
                  }

                  break;
               case 'r':
                  if (escaped)
                  {
                     builder.Append("\r");
                     escaped = false;
                  }
                  else
                  {
                     builder.Append('r');
                  }

                  break;
               case 'n':
                  if (escaped)
                  {
                     builder.Append("\n");
                     escaped = false;
                  }
                  else
                  {
                     builder.Append('n');
                  }

                  break;
               default:
                  if (current == quote)
                  {
                     if (escaped)
                     {
                        builder.Append(current);
                        escaped = false;
                     }
                     else
                     {
                        var newSource = source.Drop(i + 1);
                        var str = builder.ToString();
                        return (newSource, str, false);
                     }
                  }
                  else
                  {
                     builder.Append(current);
                     escaped = false;
                  }

                  break;
            }
         }

         return fail("Open string");
      }

      public static string GenerateKey() => $"__$key_{uniqueID()}";

      public static string GetKey(string keySource) => keySource == "?" ? GenerateKey() : keySource;
   }
}