using System.IO;
using System.Text;
using Core.Assertions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Io.Delimited
{
   public class Buffer
   {
      TextReader reader;
      int bufferSize;
      char delimiter;
      char[] buffer;
      int length;
      int startIndex;
      int index;
      bool endOfLine;
      bool endOfFile;
      StringBuilder field;

      public Buffer(TextReader reader, int bufferSize, char delimiter)
      {
         this.reader = assert(() => (object)reader).Must().Not.BeNull().Force<TextReader>();
         this.bufferSize = bufferSize.Must().BeGreaterThan(0).Force();
         this.delimiter = delimiter;
         buffer = new char[this.bufferSize];
         length = -1;
         startIndex = 0;
         index = 0;
         endOfLine = false;
         endOfFile = false;
         field = new StringBuilder();
      }

      bool getMore(bool appendToField = true)
      {
         if (endOfFile)
         {
            return false;
         }

         if (length == -1 || index >= length)
         {
            if (length > -1 && appendToField)
            {
               field.Append(buffer, startIndex, length - startIndex);
               startIndex = 0;
            }

            index = 0;
            length = reader.Read(buffer, index, bufferSize);
            if (length == 0)
            {
               endOfFile = true;
               return false;
            }
         }

         return true;
      }

      public string NextField()
      {
         endOfLine = false;
         while (getMore())
         {
            var ch = buffer[index];
            if (ch == delimiter)
            {
               var nextField = getString();
               index++;
               startIndex = index;

               return nextField;
            }

            if (ch == '\r' || ch == '\n')
            {
               var nextField = getString();
               endOfLine = true;
               while ((ch == '\r' || ch == '\n') && getMore(false))
               {
                  ch = buffer[index++];
               }

               startIndex = --index;

               return nextField;
            }

            index++;
         }

         return getString();
      }

      string getString()
      {
         var strLength = index - startIndex;
         if (strLength < 0)
         {
            return "";
         }

         if (field.Length > 0)
         {
            field.Append(buffer, startIndex, strLength);
            var result = field.ToString();
            field.Clear();

            return result;
         }

         return new string(buffer, startIndex, strLength);
      }

      public bool EndOfLine
      {
         get => endOfLine;
         set => endOfLine = value;
      }

      public bool EndOfFile => endOfFile;
   }
}