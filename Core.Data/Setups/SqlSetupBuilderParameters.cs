using System;
using Core.Computers;

namespace Core.Data.Setups;

public class SqlSetupBuilderParameters
{
   public static class Functions
   {
      public static ConnectionString connectionString(string value) => new(value);

      public static Server server(string value) => new(value);

      public static Database database(string value) => new(value);

      public static ApplicationName applicationName(string value) => new(value);

      public static User user(string value) => new(value);

      public static Password password(string value) => new(value);

      public static CommandText commandText(string value) => new(value);

      public static ParameterName parameter(string value) => new(value);

      public static FieldName field(string value) => new(value);

      public static Signature signature(string value) => new(value);

      public static ValueParameter value(string value) => new(value);

      public static DefaultValue defaultValue(string value) => new(value);

      public static ConnectionTimeout connectionTimeout(TimeSpan value) => new(value);

      public static CommandTimeout commandTimeout(TimeSpan value) => new(value);

      public static ReadOnly readOnly(bool value) => new(value);

      public static Optional optional(bool value) => new(value);

      public static Output output(bool value) => new(value);

      public static CommandTextFile commandTextFile(FileName value) => new(value);

      public static Type type(System.Type value) => new(value);

      public static Size size(int value) => new(value);
   }

   public abstract class BaseParameter
   {
   }

   public interface IConnectionStringParameter
   {
   }

   public interface ICommandTextParameter
   {
   }

   public interface IFieldParameter
   {
   }

   public interface IParameterParameter
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

   public sealed class ConnectionString : StringParameter, IConnectionStringParameter
   {
      public ConnectionString(string value) : base(value)
      {
      }
   }

   public sealed class Server : StringParameter, IConnectionStringParameter
   {
      public Server(string value) : base(value)
      {
      }
   }

   public sealed class Database : StringParameter, IConnectionStringParameter
   {
      public Database(string value) : base(value)
      {
      }
   }

   public sealed class ApplicationName : StringParameter, IConnectionStringParameter
   {
      public ApplicationName(string value) : base(value)
      {
      }
   }

   public sealed class User : StringParameter, IConnectionStringParameter
   {
      public User(string value) : base(value)
      {
      }
   }

   public sealed class Password : StringParameter, IConnectionStringParameter
   {
      public Password(string value) : base(value)
      {
      }
   }

   public sealed class CommandText : StringParameter, ICommandTextParameter
   {
      public CommandText(string value) : base(value)
      {
      }
   }

   public sealed class ParameterName : StringParameter, IParameterParameter
   {
      public ParameterName(string value) : base(value)
      {
      }
   }

   public sealed class FieldName : StringParameter, IFieldParameter
   {
      public FieldName(string value) : base(value)
      {
      }
   }

   public sealed class Signature : StringParameter, IFieldParameter, IParameterParameter
   {
      public Signature(string value) : base(value)
      {
      }
   }

   public sealed class ValueParameter : StringParameter, IParameterParameter
   {
      public ValueParameter(string value) : base(value)
      {
      }
   }

   public sealed class DefaultValue : StringParameter, IParameterParameter
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

   public sealed class ConnectionTimeout : TimeSpanParameter, IConnectionStringParameter
   {
      public ConnectionTimeout(TimeSpan value) : base(value)
      {
      }
   }

   public sealed class CommandTimeout : TimeSpanParameter, ICommandTextParameter
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

   public sealed class ReadOnly : BooleanParameter, IConnectionStringParameter
   {
      public ReadOnly(bool value) : base(value)
      {
      }
   }

   public sealed class Optional : BooleanParameter, IFieldParameter
   {
      public Optional(bool value) : base(value)
      {
      }
   }

   public sealed class Output : BooleanParameter, IParameterParameter
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

   public sealed class CommandTextFile : FileNameParameter, ICommandTextParameter
   {
      public CommandTextFile(FileName value) : base(value)
      {
      }
   }

   public abstract class TypeParameter : BaseParameter
   {
      public static implicit operator System.Type(TypeParameter type) => type.Value;

      protected TypeParameter(System.Type value)
      {
         Value = value;
      }

      public System.Type Value { get; }
   }

   public sealed class Type : TypeParameter, IParameterParameter, IFieldParameter
   {
      public Type(System.Type value) : base(value)
      {
      }
   }

   public abstract class IntParameter : BaseParameter
   {
      public static implicit operator int(IntParameter parameter) => parameter.Value;

      protected IntParameter(int value)
      {
         Value = value;
      }

      public int Value { get; }
   }

   public sealed class Size : IntParameter, IParameterParameter
   {
      public Size(int value) : base(value)
      {
      }
   }
}