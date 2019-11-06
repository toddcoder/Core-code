using System;
using System.Text;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Strings;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JsonParser
   {
      string source;
      Hash<string, string> replacements;
      TokenType lookAheadToken;
      int index;
      StringBuilder buffer;
      JsonBuilder builder;

      public event EventHandler<ParseValueArgs> ParseValue;

      public JsonParser(string source)
      {
         this.source = source;
         lookAheadToken = TokenType.None;
         index = 0;
         buffer = new StringBuilder();
         replacements = new Hash<string, string>();
         builder = new JsonBuilder();
      }

      public JsonParser(string source, Hash<string, string> replacements) : this(source)
      {
         this.replacements.Copy(replacements);
         this.source = source;
      }

      public IResult<JsonObject> Parse() =>
         from token in getToken()
         from objectOpen in token.Must().Equal(TokenType.ObjectOpen).Try(() => "Must begin with an open object")
         from member in parseMembers("")
         select builder.Root;

      protected void invokeEvent(TokenType tokenType, string name, string value)
      {
         ParseValue?.Invoke(this, new ParseValueArgs(tokenType, name, value));
      }

      void advanceIndex(int by = 1) => index += by;

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
                  if (parseString(name, '"').ValueOrCast(out var s, out original))
                  {
                     builder.Add(name, s);
                     return Unit.Success();
                  }
                  else
                  {
                     return original;
                  }
               case TokenType.StringSingle:
                  if (parseString(name, '\'').ValueOrCast(out s, out original))
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
                  setLookAheadTokenToNone();
                  builder.Add(name, true);
                  invokeEvent(TokenType.True, name, "true");

                  return Unit.Success();
               case TokenType.False:
                  setLookAheadTokenToNone();
                  builder.Add(name, false);
                  invokeEvent(TokenType.False, name, "false");

                  return Unit.Success();
               case TokenType.Null:
                  setLookAheadTokenToNone();
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
         setLookAheadTokenToNone();

         buffer.Clear();

         var length = source.Length;
         while (index < length)
         {
            var c = source[index];
            if (char.IsLetterOrDigit(c) || c == '_')
            {
               buffer.Append(c);
            }
            else
            {
               return buffer.ToString().Success();
            }

            advanceIndex();
         }

         return "Cannot end in a name".Failure<string>();
      }

      IResult<string> parseString(string name, char quote)
      {
         setLookAheadTokenToNone();

         buffer.Clear();

         advanceIndex();

         var runIndex = -1;
         var length = source.Length;
         var p = source;
         while (index < length)
         {
            var c = p[index];
            if (c == quote)
            {
               if (runIndex != -1)
               {
                  if (buffer.Length == 0)
                  {
                     advanceIndex();
                     var value = source.Drop(runIndex).Keep(index - runIndex - 1);
                     invokeEvent(TokenType.String, name, value);

                     return value.Success();
                  }

                  buffer.Append(source, runIndex, index - runIndex);
               }

               var str = replacements.Format(buffer.ToString());

               invokeEvent(TokenType.String, name, str);

               advanceIndex();

               return str.Success();
            }

            if (c != '\\')
            {
               if (runIndex == -1)
               {
                  runIndex = index;
               }

               advanceIndex();
               continue;
            }

            if (index == 1)
            {
               break;
            }

            if (runIndex != -1)
            {
               buffer.Append(source, runIndex, index - runIndex);
               runIndex = -1;
            }

            switch (p[index])
            {
               case '"':
                  buffer.Append('"');
                  break;
               case '\'':
                  buffer.Append("'");
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
                     advanceIndex(4);
                     continue;
                  }
                  else
                  {
                     return original;
                  }
            }

            advanceIndex();
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
               return parseString("", '"');
            case TokenType.StringSingle:
               return parseString("", '\'');
            default:
               return $"Invalid token @ {index}: {source.Drop(index)}".Failure<string>();
         }
      }

      IResult<Unit> parseObject(string name)
      {
         builder.BeginObject(name);

         invokeEvent(TokenType.ObjectOpen, name, "");

         setLookAheadTokenToNone();

         return parseMembers(name);
      }

      IResult<Unit> parseMembers(string name)
      {
         while (index < source.Length)
         {
            var token = lookAhead();
            if (token.ValueOrCast<Unit>(out var type, out var asUnit))
            {
               switch (type)
               {
                  case TokenType.Comma:
                     setLookAheadTokenToNone();
                     invokeEvent(TokenType.Comma, name, ",");
                     break;
                  case TokenType.ObjectClose:
                     setLookAheadTokenToNone();
                     invokeEvent(TokenType.ObjectClose, name, "}");
                     builder.End();
                     return Unit.Success();
                  default:
                     var result =
                        from innerName in parseNameOrString()
                        from next in nextToken()
                        from colon in next.Must().Equal(TokenType.Colon).Try(() => $"Expected colon at {index}: {source.Drop(index)}").Map(t => next)
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

         return "Didn't understand".Failure<Unit>();
      }

      IResult<Unit> parseArray(string name)
      {
         builder.BeginArray(name);

         invokeEvent(TokenType.ArrayOpen, name, "{");

         setLookAheadTokenToNone();

         while (true)
         {
            if (lookAhead().ValueOrCast<Unit>(out var type, out var asUnit))
            {
               switch (type)
               {
                  case TokenType.Comma:
                     setLookAheadTokenToNone();
                     invokeEvent(TokenType.Comma, name, ",");
                     break;
                  case TokenType.ArrayClose:
                     setLookAheadTokenToNone();
                     builder.End();
                     invokeEvent(TokenType.ArrayClose, name, "");

                     return Unit.Success();
                  default:
                     if (parseValue("").ValueOrOriginal(out _, out var original))
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
         setLookAheadTokenToNone();

         var startIndex = index;
         var isDecimal = false;

         while (index < source.Length)
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

               if (index >= source.Length)
               {
                  return "Unexpected end of number".Failure<object>();
               }

               advanceIndex();
               continue;
            }

            break;
         }

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

      void setLookAheadTokenToNone() => lookAheadToken = TokenType.None;

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

      void ignoreWhitespace()
      {
         if (index >= source.Length)
         {
            return;
         }

         do
         {
            var c = source[index];
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
      }

      IResult<TokenType> getToken()
      {
         ignoreWhitespace();

         if (index == source.Length)
         {
            return "Open string".Failure<TokenType>();
         }

         var c = source[index];
         switch (c)
         {
            case '{':
               advanceIndex();
               return TokenType.ObjectOpen.Success();
            case '}':
               advanceIndex();
               return TokenType.ObjectClose.Success();
            case '[':
               advanceIndex();
               return TokenType.ArrayOpen.Success();
            case ']':
               advanceIndex();
               return TokenType.ArrayClose.Success();
            case ',':
               advanceIndex();
               return TokenType.Comma.Success();
            case '"':
               return TokenType.String.Success();
            case '\'':
               return TokenType.StringSingle.Success();
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
               advanceIndex();
               return TokenType.Colon.Success();
            case 'f':
               if (source.Drop(index).Keep(5) == "false")
               {
                  advanceIndex(5);
                  return TokenType.False.Success();
               }
               else
               {
                  return TokenType.Name.Success();
               }
            case 't':
               if (source.Drop(index).Keep(4) == "true")
               {
                  advanceIndex(4);
                  return TokenType.True.Success();
               }
               else
               {
                  return TokenType.Name.Success();
               }
            case 'n':
               if (source.Drop(index).Keep(4) == "null")
               {
                  advanceIndex(4);
                  return TokenType.Null.Success();
               }
               else
               {
                  return TokenType.Name.Success();
               }
            default:
               if (char.IsLetter(c))
               {
                  return TokenType.Name.Success();
               }

               return $"Didn't expect character '{c}'".Failure<TokenType>();
         }
      }
   }
}