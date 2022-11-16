using System.Collections.Generic;
using System.Text;
using Core.DataStructures;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.Lazy.LazyMonadFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Configurations;

internal class Parser
{
   protected const string REGEX_KEY = "/(['$@']? [/w '?'] [/w '-']*)";

   protected string source;

   internal Parser(string source)
   {
      this.source = source;
   }

   public Result<Setting> Parse()
   {
      var rootSetting = new Setting();
      var stack = new MaybeStack<ConfigurationItem>();
      stack.Push(rootSetting);

      Maybe<Setting> peekSetting()
      {
         return
            from parentItem in stack.Peek()
            from parentGroup in parentItem.IfCast<Setting>()
            select parentGroup;
      }

      Maybe<Setting> popSetting()
      {
         return
            from parentItem in stack.Pop()
            from parentGroup in parentItem.IfCast<Setting>()
            select parentGroup;
      }

      Result<(string, string, bool)> getLinesAsArray(string source)
      {
         var lines = new List<string>();
         while (source.Length > 0)
         {
            var _length = source.Matches("^ /s* '}' ([/r /n]+ | $); f").Map(r => r.Length);
            var _result = lazy.maybe(() => source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f"));

            if (_length)
            {
               return (source.Drop(_length), lines.ToString(","), true);
            }
            else if (_result)
            {
               var result = ~_result;
               var _stringInfo = getString(result.FirstGroup);
               if (_stringInfo)
               {
                  var (_, @string, _) = ~_stringInfo;
                  source = source.Drop(result.Length);
                  lines.Add(@string);
               }
               else
               {
                  return _stringInfo.Exception;
               }
            }
         }

         return fail("No terminating }");
      }

      Result<(string newSource, string str, bool isArray)> getString(string source)
      {
         var _result1 = source.Matches("^ /s* /[quote]; f");
         var _result2 = lazy.maybe(() => source.Matches("^ /s* '{' [/r /n]+; f"));
         var _result3 = lazy.maybe(() => source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f"));

         if (_result1)
         {
            var result = ~_result1;
            var quote = result.FirstGroup[0];

            return getQuotedString(source.Drop(result.Length), quote);
         }
         else if (_result2)
         {
            var result = ~_result2;
            var newSource = source.Drop(result.Length);

            return getLinesAsArray(newSource);
         }
         else if (_result3)
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

      static Result<(string newSource, string str, bool isArray)> getQuotedString(string source, char quote)
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

      while (source.Length > 0)
      {
         var _openSetting = source.Matches("^ /s* '['; f").Map(r => r.Length);
         var _settingKey = lazy.maybe(() => source.Matches($"^ /s* {REGEX_KEY} /s* '['; f").Map(r => (r.FirstGroup, r.Length)));
         var _closeSetting = lazy.maybe(() => source.Matches("^ /s* ']'; f").Map(r => r.Length));
         var _oneLineKey = lazy.maybe(() => source.Matches($"^ /s* {REGEX_KEY} '.'; f").Map(r => (r.FirstGroup, r.Length)));
         var _comment = lazy.maybe(() => source.Matches("^ /s* '#' -[/r /n]*; f").Map(r => r.Length));
         var _key = lazy.maybe(() => source.Matches($"^ /s* {REGEX_KEY} ':' /s*; f").Map(r => (r.FirstGroup, r.Length)));
         var _string = lazy.result(() => getString(source.TrimLeft()));
         if (_openSetting)
         {
            var key = GetKey("?");
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting)
            {
               (~_parentSetting).SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            stack.Push(setting);

            source = source.Drop(_openSetting);
         }
         else if (_settingKey)
         {
            var (settingKey, length) = ~_settingKey;
            var key = GetKey(settingKey);
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting)
            {
               (~_parentSetting).SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            stack.Push(setting);

            source = source.Drop(length);
         }
         else if (_closeSetting)
         {
            var _setting = popSetting();
            if (_setting)
            {
               var _parentSetting = peekSetting();
               if (_parentSetting)
               {
                  (~_parentSetting).SetItem((~_setting).Key, _setting);
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

            source = source.Drop(_closeSetting);
         }
         else if (_oneLineKey)
         {
            var (oneLineKey, length) = ~_oneLineKey;
            var key = GetKey(oneLineKey);
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting)
            {
               (~_parentSetting).SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            source = source.Drop(length);
         }
         else if (_comment)
         {
            var key = GenerateKey();
            var remainder = source.Drop(_openSetting);
            var _stringTuple = getString(remainder);
            if (_stringTuple)
            {
               var (aSource, value, isArray) = ~_stringTuple;
               source = aSource;
               var item = new Item(key, value)
               {
                  IsArray = isArray
               };
               var _setting = peekSetting();
               if (_setting)
               {
                  (~_setting).SetItem(item.Key, item);
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
         else if (_key)
         {
            var (key, length) = ~_key;
            key = GetKey(key);
            var remainder = source.Drop(length);
            var _tupleString = getString(remainder);
            if (_tupleString)
            {
               var (aSource, value, isArray) = ~_tupleString;
               source = aSource;
               var item = new Item(key, value)
               {
                  IsArray = isArray
               };
               var _setting = peekSetting();
               if (_setting)
               {
                  (~_setting).SetItem(item.Key, item);
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
         else if (_string)
         {
            var (aSource, value, isArray) = ~_string;
            source = aSource;
            var key = GenerateKey();
            var item = new Item(key, value)
            {
               IsArray = isArray
            };
            var _setting = peekSetting();
            if (_setting)
            {
               (~_setting).SetItem(item.Key, item);
            }
         }
         else
         {
            return fail($"Didn't understand {source.KeepUntil("\r\n")}");
         }
      }

      while (true)
      {
         var _setting = popSetting();
         if (!_setting)
         {
            break;
         }

         var _parentSetting = popSetting();
         if (_parentSetting)
         {
            (~_parentSetting).SetItem((~_setting).Key, _setting);
         }
         else
         {
            break;
         }
      }

      return rootSetting;
   }

   public static string GenerateKey() => $"__$key_{uniqueID()}";

   public static string GetKey(string keySource) => keySource == "?" ? GenerateKey() : keySource;
}