using System;
using Core.Computers;

namespace Core.Data.Setups;

public class SqlSetupBuilderParameters
{
   public static class Functions
   {
      public static StringParameter connectionString(string value) => new ConnectionString(value);

      public static StringParameter server(string value) => new Server(value);

      public static StringParameter database(string value) => new Database(value);

      public static StringParameter applicationName(string value) => new ApplicationName(value);

      public static StringParameter commandText(string value) => new CommandText(value);

      public static StringParameter parameter(string value) => new Parameter(value);

      public static StringParameter field(string value) => new Field(value);

      public static StringParameter signature(string value) => new Signature(value);

      public static StringParameter value(string value) => new ValueParameter(value);

      public static StringParameter defaultValue(string value) => new DefaultValue(value);

      public static TimeSpanParameter connectionTimeout(TimeSpan value) => new ConnectionTimeout(value);

      public static TimeSpanParameter commandTimeout(TimeSpan value) => new CommandTimeout(value);

      public static BooleanParameter readOnly(bool value) => new ReadOnly(value);

      public static BooleanParameter optional(bool value) => new ReadOnly(value);

      public static BooleanParameter output(bool value) => new Output(value);

      public static FileNameParameter commandTextFile(FileName value) => new CommandTextFile(value);

      public static TypeParameter type(System.Type value) => new Type(value);

      public static IntParameter size(int value) => new Size(value);
   }

   public abstract class BaseParameter
   {
   }

   public abstract class StringParameter : BaseParameter
   {
      public static implicit operator string(StringParameter parameter) => parameter.Value;

      public StringParameter(string value)
      {
         Value = value;
      }

      public string Value { get; }
   }

   public sealed class ConnectionString : StringParameter
   {
      public ConnectionString(string value) : base(value)
      {
      }
   }

   public sealed class Server : StringParameter
   {
      public Server(string value) : base(value)
      {
      }
   }

   public sealed class Database : StringParameter
   {
      public Database(string value) : base(value)
      {
      }
   }

   public sealed class ApplicationName : StringParameter
   {
      public ApplicationName(string value) : base(value)
      {
      }
   }

   public sealed class CommandText : StringParameter
   {
      public CommandText(string value) : base(value)
      {
      }
   }

   public sealed class Parameter : StringParameter
   {
      public Parameter(string value) : base(value)
      {
      }
   }

   public sealed class Field : StringParameter
   {
      public Field(string value) : base(value)
      {
      }
   }

   public sealed class Signature : StringParameter
   {
      public Signature(string value) : base(value)
      {
      }
   }

   public sealed class ValueParameter : StringParameter
   {
      public ValueParameter(string value) : base(value)
      {
      }
   }

   public sealed class DefaultValue : StringParameter
   {
      public DefaultValue(string value) : base(value)
      {
      }
   }

   public abstract class TimeSpanParameter : BaseParameter
   {
      public static implicit operator TimeSpan(TimeSpanParameter parameter) => parameter.Value;

      protected TimeSpanParameter(TimeSpan value)
      {
         Value = value;
      }

      public TimeSpan Value { get; }
   }

   public sealed class ConnectionTimeout : TimeSpanParameter
   {
      public ConnectionTimeout(TimeSpan value) : base(value)
      {
      }
   }

   public sealed class CommandTimeout : TimeSpanParameter
   {
      public CommandTimeout(TimeSpan value) : base(value)
      {
      }
   }

   public abstract class BooleanParameter : BaseParameter
   {
      public static implicit operator bool(BooleanParameter parameter) => parameter.Value;

      protected BooleanParameter(bool value)
      {
         Value = value;
      }

      public bool Value { get; }
   }

   public sealed class ReadOnly : BooleanParameter
   {
      public ReadOnly(bool value) : base(value)
      {
      }
   }

   public sealed class Optional : BooleanParameter
   {
      public Optional(bool value) : base(value)
      {
      }
   }

   public sealed class Output : BooleanParameter
   {
      public Output(bool value) : base(value)
      {
      }
   }

   public abstract class FileNameParameter : BaseParameter
   {
      public static implicit operator FileName(FileNameParameter parameter) => parameter.Value;

      protected FileNameParameter(FileName value)
      {
         Value = value;
      }

      public FileName Value { get; }
   }

   public sealed class CommandTextFile : FileNameParameter
   {
      public CommandTextFile(FileName value) : base(value)
      {
      }
   }

   public abstract class TypeParameter : BaseParameter
   {
      protected TypeParameter(System.Type value)
      {
         Value = value;
      }

      public System.Type Value { get; }
   }

   public sealed class Type : TypeParameter
   {
      public Type(System.Type value) : base(value)
      {
      }
   }

   public abstract class IntParameter : BaseParameter
   {
      protected IntParameter(int value)
      {
         Value = value;
      }

      public int Value { get; }
   }

   public sealed class Size : IntParameter
   {
      public Size(int value) : base(value)
      {
      }
   }
}