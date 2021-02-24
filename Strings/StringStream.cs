using System.Reflection;
using System.Text;

namespace Core.Strings
{
   public class StringStream
   {
      public static string EndLine => "\r\n";

      public static StringStream operator /(StringStream stream, StringStream other) => stream.append(other, false);

      public static StringStream operator /(StringStream stream, IStringStream other) => stream.append(other.ToStream(), false);

      public static StringStream operator /(StringStream stream, string text) => stream.append(text, false);

      public static StringStream operator /(StringStream stream, (bool, string) conditional)
      {
         var (test, result) = conditional;
         if (test)
         {
            return stream.append(result, false);
         }
         else
         {
            return stream;
         }
      }

      public static StringStream operator /(StringStream stream, object obj) => stream.append(obj, false);

      public static StringStream operator %(StringStream stream, StringStream other) => stream.append(other, true);

      public static StringStream operator %(StringStream stream, IStringStream other) => stream.append(other, true);

      public static StringStream operator %(StringStream stream, string text) => stream.append(text, true);

      public static StringStream operator %(StringStream stream, (bool, string) conditional)
      {
         var (test, result) = conditional;
         if (test)
         {
            return stream.append(result, true);
         }
         else
         {
            return stream;
         }
      }

      public static StringStream operator %(StringStream stream, object obj) => stream.append(obj, true);

      public static implicit operator string(StringStream stream) => stream.ToString();

      public static explicit operator StringStream(string value) => new StringStream().append(value, false);

      protected StringBuilder builder;

      public StringStream() => builder = new StringBuilder();

      protected StringStream append(string text, bool endLine)
      {
         if (endLine)
         {
            builder.Append(EndLine);
         }

         builder.Append(text);

         return this;
      }

      protected StringStream append(object obj, bool endLine)
      {
         if (!(obj is null))
         {
            var type = obj.GetType();
            switch (type.Name)
            {
               case "Some`1":
               case "Success`1":
                  var value = type.InvokeMember("Value", BindingFlags.GetProperty, null, obj, null).ToNonNullString();
                  return append(value, endLine);
               case "None`1":
               case "Failure`1":
                  return this;
               default:
                  return append(obj.ToNonNullString(), endLine);
            }
         }
         else
         {
            return this;
         }
      }

      public override string ToString() => builder.ToString();
   }
}