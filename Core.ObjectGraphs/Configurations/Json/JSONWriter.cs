using System;
using System.IO;
using Core.Arrays;
using Core.Computers;
using Core.Objects;
using Core.Strings;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JSONWriter
   {
      StringWriter writer;
      string indentString;
      string indentation;
      int indentLevel;
      bool opened;
      bool nameWritten;

      public JSONWriter(string indentString = "   ", int indentLevel = 0)
      {
         writer = new StringWriter();
         this.indentString = indentString;
         this.indentLevel = indentLevel;
         opened = false;
         nameWritten = true;
      }

      public string IndentString => indentString;

      public int IndentLevel => indentLevel;

      void indent()
      {
         indentation = indentString.Repeat(++indentLevel);
      }

      void unindent()
      {
         if (indentLevel > 0)
         {
            indentation = indentString.Repeat(--indentLevel);
         }
      }

      void writeOpen(string ch)
      {
         if (nameWritten)
         {
            if (opened)
            {
               writer.WriteLine();
               indent();
               writer.Write(indentation);
            }

            Write(ch);
            if (!opened)
            {
               indent();
               opened = true;
            }
         }
         else
         {
            if (opened)
            {
               writer.WriteLine();
            }
            else
            {
               writer.WriteLine(",");
            }

            writer.Write($"{indentation}{ch}");
            indent();
            opened = true;
         }
      }

      void writeEnd(string ch)
      {
         writer.WriteLine();
         unindent();
         writer.Write($"{indentation}{ch}");
      }

      public void BeginObject() => writeOpen("{");

      public void EndObject() => writeEnd("}");

      public void BeginArray() => writeOpen("[");

      public void EndArray() => writeEnd("]");

      void writeName(string name)
      {
         if (opened)
         {
            opened = false;
            writer.WriteLine();
         }
         else
         {
            writer.WriteLine(",");
         }

         writer.Write(name.IsNotEmpty() ? $"{indentation}{name.Quotify()}: " : indentation);
         nameWritten = true;
      }

      public void WriteName(string name) => writeName(name);

      public void Write(string value)
      {
         writer.Write(value);
         nameWritten = false;
      }

      public void WriteQuoted(string value) => Write(value.Quotify());

      public void Write(string name, string value)
      {
         writeName(name);
         WriteQuoted(value);
      }

      public void Write(bool value) => Write(value.ToString().ToLower());

      public void Write(string name, bool value)
      {
         writeName(name);
         Write(value);
      }

      public void Write(byte[] value) => WriteQuoted(value.ToBase64());

      public void Write(string name, byte[] value)
      {
         writeName(name);
         Write(value);
      }

      public void Write(FileName fileName) => WriteQuoted(fileName.ToString().Replace(@"\", @"\\"));

      public void Write(string name, FileName fileName)
      {
         writeName(name);
         Write(fileName);
      }

      public void Write(FolderName folderName) => WriteQuoted(folderName.ToString().Replace(@"\", @"\\"));

      public void Write(string name, FolderName folderName)
      {
         writeName(name);
         Write(folderName);
      }

      public void Write(Enum value) => WriteQuoted(value.ToString());

      public void Write(string name, Enum value)
      {
         writeName(name);
         Write(value);
      }

      public void Write(string name, int integer)
      {
         writeName(name);
         Write(integer);
      }

      public void Write(int integer) => Write(integer.ToString());

      public void Write(string name, double @double)
      {
         writeName(name);
         Write(@double);
      }

      public void Write(double @double)
      {
         var result = @double.ToString();
         Write(result.Contains(".") ? result : $"{result}.0");
      }

      public void Write(object value)
      {
         if (value.IsNull())
         {
            Write("null");
         }
         else
         {
            WriteQuoted(value.ToString());
         }
      }

      public void Write(string name, object value)
      {
         writeName(name);
         Write(value);
      }

      public override string ToString() => writer.ToString();
   }
}