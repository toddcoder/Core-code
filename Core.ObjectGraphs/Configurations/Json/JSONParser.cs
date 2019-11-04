using System;
using System.Text;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Strings;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JSONParser
   {
      string source;
      Hash<string, string> replacements;
      TokenType lookAheadToken;
      int index;
      StringBuilder buffer;
      JSONBuilder builder;

      public event EventHandler<ParseValueArgs> ParseValue;

      public JSONParser(string source)
      {
         this.source = source;
         lookAheadToken = TokenType.None;
         index = 0;
         buffer = new StringBuilder();
         replacements = new Hash<string, string>();
         builder = new JSONBuilder();
      }

      public JSONParser(string source, Hash<string, string> replacements) : this(source)
      {
         this.replacements.Copy(replacements);
         this.source = source;
      }

      public IResult<JSONObject> Parse() =>
         from token in getToken()
         from members in parseMembers("")
         select builder.Root;

      protected void invokeEvent(TokenType tokenType, string name, string value)
      {
         ParseValue?.Invoke(this, new ParseValueArgs(tokenType, name, value));
      }

      IResult<Unit> parseValue(string name)
      {
         var anyTokenType = lookAhead();
         if (anyTokenType.ValueOrCast<Unit>(out var tokenType, out var original))
         {
            switch (tokenType)
            {
               case TokenType.Number:
                  if (parseNumber(name).ValueOrCast(out var number, out original))
                  {
                     switch (number)
                     {
                        case double d:
                           builder.Add(name, d);
                           return Unit.Success();
                        case int i:
                           builder.Add(name, i);
                           return Unit.Success();
                        default:
                           return $"Couldn't determine number type of {number}".Failure<Unit>();
                     }
                  }
                  else
                  {
                     return original;
                  }
               case TokenType.String:
                  if (parseString(name).ValueOrCast(out var s, out original))
                  {
                     builder.Add(name, s);
                     return Unit.Success();
                  }
                  else
                  {
                     return original;
                  }
               case TokenType.ObjectOpen:
                  return parseObject(name);
               case TokenType.ArrayOpen:
                  return parseArray(name);
               case TokenType.True:
                  consumeToken();
                  builder.Add(name, true);
                  invokeEvent(TokenType.True, name, "true");

                  return Unit.Success();
               case TokenType.False:
                  consumeToken();
                  builder.Add(name, false);
                  invokeEvent(TokenType.False, name, "false");

                  return Unit.Success();
               case TokenType.Null:
                  consumeToken();
                  builder.Add(name);
                  invokeEvent(TokenType.Null, name, "null");

                  return Unit.Success();
               default:
                  return $"Didn't understand token at {index}".Failure<Unit>();
            }
         }
         else
         {
            return original;
         }
      }

      IResult<string> parseName()
      {
         consumeToken();

         buffer.Clear();

         var length = source.Length;
         while (index < length)
         {
            var c = source[index++];
            if (char.IsLetter(c))
            {
               buffer.Append(c);
            }
            else
            {
               return buffer.ToString().Success();
            }
         }

         return "Cannot end in a name".Failure<string>();
      }

      IResult<string> parseString(string name)
      {
         consumeToken();

         buffer.Clear();

         var runIndex = -1;
         var length = source.Length;
         var p = source;
         while (index < length)
         {
            var c = p[index++];
            if (c == '"')
            {
               if (runIndex != -1)
               {
                  if (buffer.Length == 0)
                  {
                     return source.Drop(runIndex).Keep(index - runIndex - 1).Success();
                  }

                  buffer.Append(source, runIndex, index - runIndex - 1);
               }

               var str = replacements.Format(buffer.ToString());

               invokeEvent(TokenType.String, name, str);

               return str.Success();
            }

            if (c != '\\')
            {
               if (runIndex == -1)
               {
                  runIndex = index - 1;
               }

               continue;
            }

            if (index == 1)
            {
               break;
            }

            if (runIndex != -1)
            {
               buffer.Append(source, runIndex, index - runIndex - 1);
               runIndex = -1;
            }

            switch (p[index++])
            {
               case '"':
                  buffer.Append('"');
                  break;
               case '\\':
                  buffer.Append('\\');
                  break;
               case '/':
                  buffer.Append('/');
                  break;
               case 'b':
                  buffer.Append('\b');
                  break;
               case 'f':
                  buffer.Append('\f');
                  break;
               case 'n':
                  buffer.Append('\n');
                  break;
               case 'r':
                  buffer.Append('\r');
                  break;
               case 't':
                  buffer.Append('\t');
                  break;
               case 'u':
                  var remainingLength = 1 - index;
                  if (remainingLength < 4)
                  {
                     return @"Malformed \u".Failure<string>();
                  }
                  else if (parseUnicode(p[index], p[index + 1], p[index + 2], p[index + 3]).ValueOrCast<string>(out var codePoint, out var original))
                  {
                     buffer.Append((char)codePoint);
                     index += 4;
                     break;
                  }
                  else
                  {
                     return original;
                  }
            }
         }

         return "Open string".Failure<string>();
      }

      IResult<string> parseNameOrString()
      {
         switch (lookAheadToken)
         {
            case TokenType.Name:
               return parseName();
            case TokenType.String:
               return parseString("");
            default:
               return "Invalid token".Failure<string>();
         }
      }

      IResult<Unit> parseObject(string name)
      {
         builder.BeginObject(name);

         invokeEvent(TokenType.ObjectOpen, name, "");

         return parseMembers(name).OnSuccess(_ => invokeEvent(TokenType.ObjectClose, name, ""));
      }

      IResult<Unit> parseMembers(string name)
      {
         consumeToken();

         while (true)
         {
            var token = lookAhead();
            if (token.ValueOrCast<Unit>(out var type, out var asUnit))
            {
               switch (type)
               {
                  case TokenType.Comma:
                     consumeToken();
                     invokeEvent(TokenType.Comma, name, ",");
                     break;
                  case TokenType.ObjectClose:
                     consumeToken();
                     invokeEvent(TokenType.ObjectClose, name, "}");
                     builder.End();
                     return Unit.Success();
                  default:
                     var result =
                        from innerName in parseNameOrString()
                        from next in nextToken()
                        from colon in next.Must().Equal(TokenType.Colon).Try(() => $"Expected colon at {index}").Map(t => next)
                        from value in parseValue(innerName)
                        select value;
                     if (result.ValueOrOriginal(out _, out var original))
                     {
                        break;
                     }
                     else
                     {
                        return original;
                     }
               }
            }
            else
            {
               return asUnit;
            }
         }
      }

      IResult<Unit> parseArray(string name)
      {
         builder.BeginArray(name);

         invokeEvent(TokenType.ArrayOpen, name, "{");

         consumeToken();

         while (true)
         {
            if (lookAhead().ValueOrCast<Unit>(out var type, out var asUnit))
            {
               switch (type)
               {
                  case TokenType.Comma:
                     consumeToken();
                     invokeEvent(TokenType.Comma, name, ",");
                     break;
                  case TokenType.ArrayClose:
                     consumeToken();
                     builder.End();
                     invokeEvent(TokenType.ArrayClose, name, "");

                     return Unit.Success();
                  default:
                     if (parseValue(name).ValueOrOriginal(out _, out var original))
                     {
                        break;
                     }
                     else
                     {
                        return original;
                     }
               }
            }
            else
            {
               return asUnit;
            }
         }
      }

      static IResult<uint> parseSingleChar(char c, uint multiplier)
      {
         uint p;
         if (c.Between('0').And('9'))
         {
            p = (uint)(c - '0') * multiplier;
         }
         else if (c.Between('A').And('F'))
         {
            p = (uint)(c - 'A' + 10) * multiplier;
         }
         else if (c.Between('a').And('f'))
         {
            p = (uint)(c - 'A' + 10) * multiplier;
         }
         else
         {
            return $"Didn't understand {c} in Unicode string".Failure<uint>();
         }

         return p.Success();
      }

      static IResult<uint> parseUnicode(char c1, char c2, char c3, char c4) =>
         from p1 in parseSingleChar(c1, 0x100)
         from p2 in parseSingleChar(c2, 0x100)
         from p3 in parseSingleChar(c3, 0x10)
         from p4 in parseSingleChar(c4, 0)
         select p1 + p2 + p3 + p4;

      IResult<object> parseNumber(string name)
      {
         consumeToken();

         var startIndex = index - 1;
         var isDecimal = false;

         do
         {
            if (index == source.Length)
            {
               break;
            }

            var c = source[index];
            if (c.Between('0').And('9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
            {
               if (c == '.' || c == 'e' || c == 'E')
               {
                  isDecimal = true;
               }

               if (++index == source.Length)
               {
                  return "Unexpected end of number".Failure<object>();
               }

               continue;
            }

            break;
         } while (true);

         var str = source.Drop(startIndex).Keep(index - startIndex);

         invokeEvent(TokenType.Number, name, str);

         if (isDecimal)
         {
            return str.Double().Map(d => (object)d);
         }
         else
         {
            return str.Int32().Map(i => (object)i);
         }
      }

      IResult<TokenType> lookAhead()
      {
         if (lookAheadToken != TokenType.None)
         {
            return lookAheadToken.Success();
         }

         var token = getToken();
         if (token.If(out var tt))
         {
            lookAheadToken = tt;
         }

         return token;
      }

      void consumeToken() => lookAheadToken = TokenType.None;

      IResult<TokenType> nextToken()
      {
         if (lookAheadToken != TokenType.None)
         {
            return lookAheadToken.Success();
         }
         else if (getToken().ValueOrOriginal(out var type, out var original))
         {
            return type.Success();
         }
         else
         {
            return original;
         }
      }

      IResult<TokenType> getToken()
      {
         char c;

         do
         {
            c = source[index];
            if (c == '/' && source[index + 1] == '/')
            {
               index += 2;
               do
               {
                  c = source[index];
                  if (c == '\r' || c == '\n')
                  {
                     break;
                  }
               } while (++index < source.Length);
            }

            if (c > ' ')
            {
               break;
            }

            if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
            {
               break;
            }
         } while (++index < source.Length);

         if (index == source.Length)
         {
            return "Open string".Failure<TokenType>();
         }

         c = source[index++];
         switch (c)
         {
            case '{':
               return TokenType.ObjectOpen.Success();
            case '}':
               return TokenType.ObjectClose.Success();
            case '[':
               return TokenType.ArrayOpen.Success();
            case ']':
               return TokenType.ArrayClose.Success();
            case ',':
               return TokenType.Comma.Success();
            case '"':
               return TokenType.String.Success();
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case '-':
            case '+':
            case '.':
               return TokenType.Number.Success();
            case ':':
               return TokenType.Colon.Success();
            case 'f':
               if (source.Drop(index - 1).Keep(5) == "false")
               {
                  index += 5;
                  return TokenType.False.Success();
               }
               else
               {
                  return TokenType.Name.Success();
               }
            case 't':
               if (source.Drop(index - 1).Keep(4) == "true")
               {
                  index += 4;
                  return TokenType.True.Success();
               }
               else
               {
                  return TokenType.Name.Success();
               }
            case 'n':
               if (source.Drop(index - 1).Keep(4) == "null")
               {
                  index += 4;
                  return TokenType.Null.Success();
               }
               else
               {
                  return TokenType.Name.Success();
               }
         }

         return $"Didn't understand {source.Drop(--index)}".Failure<TokenType>();
      }
   }
}