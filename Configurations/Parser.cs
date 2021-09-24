using System.Text;
using Core.DataStructures;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Configurations
{
   internal class Parser
   {
      protected const string REGEX_KEY = "/(['$@']? [/w '?'] [/w '-']*)";

      protected string source;

      internal Parser(string source)
      {
         this.source = source;
      }

      public Result<Group> Parse()
      {
         var rootGroup = new Group();
         var stack = new MaybeStack<IConfigurationItem>();
         stack.Push(rootGroup);

         Maybe<Group> peekGroup()
         {
            return
               from parentItem in stack.Peek()
               from parentGroup in parentItem.IfCast<Group>()
               select parentGroup;
         }

         Maybe<Group> popGroup()
         {
            return
               from parentItem in stack.Pop()
               from parentGroup in parentItem.IfCast<Group>()
               select parentGroup;
         }

         Result<(string newSource, string str)> getString(string source)
         {
            if (source.Matches("^ /s* /[quote]; f").If(out var result))
            {
               var quote = result.FirstGroup[0];
               return getQuotedString(source.Drop(result.Length), quote);
            }
            else if (source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f").If(out result))
            {
               var foundReturn = false;
               var builder = new StringBuilder();
               for (var i = 0; i < source.Length; i++)
               {
                  var current = source[i];
                  switch (current)
                  {
                     case ';':
                        return (source.Drop(i + 1), builder.ToString()).Success();
                     case ']' or '#':
                        return (source.Drop(i), builder.ToString()).Success();
                     case '\r' or '\n':
                        foundReturn = true;
                        break;
                     default:
                        if (foundReturn)
                        {
                           return (source.Drop(i - 1), builder.ToString()).Success();
                        }
                        else
                        {
                           builder.Append(current);
                        }

                        break;
                  }
               }

               return (string.Empty, builder.ToString());
            }
            else
            {
               return fail("Couldn't determine string");
            }
         }

         static Result<(string newSource, string str)> getQuotedString(string source, char quote)
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
                           return (newSource, str);
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

         static string getKey(string keySource) => keySource == "?" ? $"__key_{uniqueID()}" : keySource;

         while (source.Length > 0)
         {
            if (source.Matches($"^ /s* {REGEX_KEY} /s* '['; f").If(out var result))
            {
               var key = getKey(result.FirstGroup);
               var group = new Group(key);
               if (peekGroup().If(out var parentGroup))
               {
                  parentGroup[key] = group;
               }
               else
               {
                  return fail("No parent group found");
               }

               stack.Push(group);

               source = source.Drop(result.Length);
            }
            else if (source.Matches("^ /s* ']'; f").If(out result))
            {
               if (popGroup().If(out var group))
               {
                  if (peekGroup().If(out var parentGroup))
                  {
                     parentGroup[group.Key] = group;
                  }
                  else
                  {
                     return fail("No parent group found");
                  }
               }
               else
               {
                  return fail("Not closing on group");
               }

               source = source.Drop(result.Length);
            }
            else if (source.Matches($"^ /s* {REGEX_KEY} '.'; f").If(out result))
            {
               var key = getKey(result.FirstGroup);
               var group = new Group(key);
               if (peekGroup().If(out var parentGroup))
               {
                  parentGroup[key] = group;
               }
               else
               {
                  return fail("No parent group found");
               }

               source = source.Drop(result.Length);
            }
            else if (source.Matches("^ /s* '#' -[/r /n]*; f").If(out result))
            {
               source = source.Drop(result.Length);
            }
            else if (source.Matches($"^ /s* {REGEX_KEY} ':' /s*; f").If(out result))
            {
               var key = getKey(result.FirstGroup);
               var remainder = source.Drop(result.Length);
               if (getString(source.Drop(result.Length)).If(out source, out var value))
               {
                  var item = new Item(key, value);
                  if (peekGroup().If(out var group))
                  {
                     group[item.Key] = item;
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
            else
            {
               return fail($"Didn't understand {source.KeepUntil("\r\n")}");
            }
         }

         while (popGroup().If(out var group))
         {
            if (peekGroup().If(out var parentGroup))
            {
               parentGroup[group.Key] = group;
            }
            else
            {
               break;
            }
         }

         return rootGroup;
      }
   }
}