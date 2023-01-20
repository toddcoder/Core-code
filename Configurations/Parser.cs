using System.Collections.Generic;
using System.Text;
using Core.DataStructures;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.Lazy.LazyMonads;
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
            var _length = lazy.maybe<int>();
            var _result = lazy.maybe<MatchResult>();

            if (_length.ValueOf(source.Matches("^ /s* '}' ([/r /n]+ | $); f").Map(r => r.Length)))
            {
               return (source.Drop(_length), lines.ToString(","), true);
            }
            else if (_result.ValueOf(source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f")))
            {
               var _stringInfo = getString(_result.Value.FirstGroup);
               if (_stringInfo)
               {
                  var (_, @string, _) = _stringInfo.Value;
                  source = source.Drop(_result.Value.Length);
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
         var _quote = lazy.maybe<MatchResult>();
         var _openBrace = lazy.maybe<MatchResult>();
         var _endOfLine = lazy.maybe<MatchResult>();

         if (_quote.ValueOf(source.Matches("^ /s* /[quote]; f")))
         {
            var quote = _quote.Value.FirstGroup[0];
            return getQuotedString(source.Drop(_quote.Value.Length), quote);
         }
         else if (_openBrace.ValueOf(source.Matches("^ /s* '{' [/r /n]+; f")))
         {
            var newSource = source.Drop(_openBrace.Value.Length);
            return getLinesAsArray(newSource);
         }
         else if (_endOfLine.ValueOf(source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f")))
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
         var _openSetting = lazy.maybe<int>();
         var _settingKey = lazy.maybe<(string, int)>();
         var _closeSetting = lazy.maybe<int>();
         var _oneLineKey = lazy.maybe<(string, int)>();
         var _comment = lazy.maybe<int>();
         var _key = lazy.maybe<(string, int)>();
         var _string = lazy.result<(string, string, bool)>();
         if (_openSetting.ValueOf(source.Matches("^ /s* '['; f").Map(r => r.Length)))
         {
            var key = GetKey("?");
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting)
            {
               _parentSetting.Value.SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            stack.Push(setting);

            source = source.Drop(_openSetting);
         }
         else if (_settingKey.ValueOf(source.Matches($"^ /s* {REGEX_KEY} /s* '['; f").Map(r => r.FirstGroupAndLength)))
         {
            var (settingKey, length) = _settingKey.Value;
            var key = GetKey(settingKey);
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting)
            {
               _parentSetting.Value.SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            stack.Push(setting);

            source = source.Drop(length);
         }
         else if (_closeSetting.ValueOf(source.Matches("^ /s* ']'; f").Map(r => r.Length)))
         {
            var _setting = popSetting();
            if (_setting)
            {
               var _parentSetting = peekSetting();
               if (_parentSetting)
               {
                  _parentSetting.Value.SetItem(_setting.Value.Key, _setting);
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
         else if (_oneLineKey.ValueOf(source.Matches($"^ /s* {REGEX_KEY} '.'; f").Map(r => r.FirstGroupAndLength)))
         {
            var (oneLineKey, length) = _oneLineKey.Value;
            var key = GetKey(oneLineKey);
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting)
            {
               _parentSetting.Value.SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            source = source.Drop(length);
         }
         else if (_comment.ValueOf(source.Matches("^ /s* '#' -[/r /n]*; f").Map(r => r.Length)))
         {
            var key = GenerateKey();
            var remainder = source.Drop(_openSetting);
            var _stringTuple = getString(remainder);
            if (_stringTuple)
            {
               var (aSource, value, isArray) = _stringTuple.Value;
               source = aSource;
               var item = new Item(key, value)
               {
                  IsArray = isArray
               };
               var _setting = peekSetting();
               if (_setting)
               {
                  _setting.Value.SetItem(item.Key, item);
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
         else if (_key.ValueOf(source.Matches($"^ /s* {REGEX_KEY} ':' /s*; f").Map(r => r.FirstGroupAndLength)))
         {
            var (key, length) = _key.Value;
            key = GetKey(key);
            var remainder = source.Drop(length);
            var _tupleString = getString(remainder);
            if (_tupleString)
            {
               var (aSource, value, isArray) = _tupleString.Value;
               source = aSource;
               var item = new Item(key, value)
               {
                  IsArray = isArray
               };
               var _setting = peekSetting();
               if (_setting)
               {
                  _setting.Value.SetItem(item.Key, item);
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
         else if (_string.ValueOf(getString(source.TrimLeft())))
         {
            var (aSource, value, isArray) = _string.Value;
            source = aSource;
            var key = GenerateKey();
            var item = new Item(key, value)
            {
               IsArray = isArray
            };
            var _setting = peekSetting();
            if (_setting)
            {
               _setting.Value.SetItem(item.Key, item);
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
            _parentSetting.Value.SetItem(_setting.Value.Key, _setting);
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