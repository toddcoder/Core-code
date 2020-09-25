﻿using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.RegularExpressions;

namespace Core.Strings
{
   public class FieldRecord
   {
      public static string DoubleQuote => "\"";

      public static string CEscapedQuote => @"\""";

      public static string VBEscapedQuote => "\"\"";

      List<string> fields;
      List<string> records;

      public FieldRecord()
      {
         FieldDelimiter = "/t";
         RecordDelimiter = "/r /n";
         Quote = DoubleQuote;
         EscapedQuote = CEscapedQuote;
         fields = new List<string>();
         records = new List<string>();
         AutoQuotify = true;
      }

      public string FieldDelimiter { get; set; }

      public string RecordDelimiter { get; set; }

      public string Quote { get; set; }

      public string EscapedQuote { get; set; }

      public bool AutoQuotify { get; set; }

      public string Text
      {
         get => ToString();
         set => fromString(value);
      }

      public int RecordCount => records.Count;

      public string[] this[int index] => records[index].Split(FieldDelimiter);

      public void WriteField(string field)
      {
         if (AutoQuotify && FieldDelimiter == "','" && field.Has(","))
         {
            fields.Add(Quote + field.Replace(Quote, EscapedQuote) + Quote);
         }
         else
         {
            fields.Add(field);
         }
      }

      public void WriteRecord()
      {
         records.Add(fields.ToString(FieldDelimiter));
         fields.Clear();
      }

      public void Clear()
      {
         fields.Clear();
         records.Clear();
      }

      public override string ToString()
      {
         if (fields.Count > 0)
         {
            WriteRecord();
         }

         return records.ToString(RecordDelimiter);
      }

      void fromString(string value)
      {
         Clear();

         var delimitedText = Quote == DoubleQuote ? DelimitedText.AsCLike() : DelimitedText.AsSql();
         var recordDelimiterEscaped = RecordDelimiter.Escape();
         var fieldDelimiterEscaped = FieldDelimiter.Escape();
         var restringifyQuotes = Quote == DoubleQuote ? RestringifyQuotes.DoubleQuote : RestringifyQuotes.SingleQuote;

         foreach (var field in value.Split(recordDelimiterEscaped).SelectMany(line => line.Split(fieldDelimiterEscaped)))
         {
            WriteField(delimitedText.Restringify(field, restringifyQuotes));
         }

         WriteRecord();
      }

      public IEnumerable<string[]> Records() => records.Select(record => record.Split(FieldDelimiter));
   }
}