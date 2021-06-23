using System;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using Core.Computers;
using Core.Dates;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using static System.Convert;
using static Core.Monads.MonadFunctions;

namespace Core.Data.DataSources
{
   public class OleDbDataSource : DataSource
   {
      protected static OleDbType typeToOleDbType(Type type) => Type.GetTypeCode(type) switch
      {
         TypeCode.Boolean => OleDbType.Boolean,
         TypeCode.Byte => OleDbType.UnsignedTinyInt,
         TypeCode.Char => OleDbType.Char,
         TypeCode.DateTime => OleDbType.DBTimeStamp,
         TypeCode.Decimal => OleDbType.Decimal,
         TypeCode.Double => OleDbType.Double,
         TypeCode.Int16 => OleDbType.SmallInt,
         TypeCode.Int32 => OleDbType.Integer,
         TypeCode.Int64 => OleDbType.BigInt,
         TypeCode.Object => OleDbType.Variant,
         TypeCode.String => OleDbType.VarWChar,
         _ => throw $"Doesn't support {type}".Throws()
      };

      protected Maybe<FileName> associatedFile;

      public OleDbDataSource(string connectionString, Maybe<FileName> associatedFile) : base(connectionString, "30 seconds".ToTimeSpan())
      {
         ConnectionString = getFileConnectionString(associatedFile);
         this.associatedFile = associatedFile;
      }

      public override IDbConnection GetConnection()
      {
         var oleDbConnection = new OleDbConnection(ConnectionString);
         oleDbConnection.Open();

         return oleDbConnection;
      }

      public override IDbCommand GetCommand() => new OleDbCommand();

      public override void AddParameters(object entity, Parameters.Parameters parameters)
      {
         Command.Required("Command has not be set").Parameters.Clear();

         foreach (var parameter in parameters)
         {
            if (parameter.Type.If(out var parameterType))
            {
            }
            else
            {
               parameter.DeterminePropertyType(entity);
               parameterType = parameter.PropertyType;
               parameter.Type = parameterType.Some();
            }

            var oledbParameter = parameter.Size
               .Map(size => new OleDbParameter(parameter.Name, typeToOleDbType(parameterType), size))
               .DefaultTo(() => new OleDbParameter(parameter.Name, typeToDBType(parameterType)));

            if (parameter.Output)
            {
               oledbParameter.Direction = ParameterDirection.Output;
            }
            else if (parameter.Value.If(out var str))
            {
               if (parameterType == typeof(string))
               {
                  oledbParameter.Value = str;
               }
               else
               {
                  var obj = str.ToObject().Required($"Couldn't convert {str}");
                  oledbParameter.Value = ChangeType(obj, parameterType);
               }
            }
            else
            {
               var value = parameter.GetValue(entity).Required($"Parameter {parameter.Name}'s value couldn't be determined");
               if (value is null && parameter.Default.If(out var defaultValue))
               {
                  value = parameter.Type.Map(t => ChangeType(defaultValue, t)).DefaultTo(() => defaultValue);
               }

               var type = value?.GetType();
               var underlyingType = type?.UnderlyingTypeOf() ?? none<Type>();
               if (underlyingType.IsSome)
               {
                  value = type.InvokeMember("Value", BindingFlags.GetProperty, null, value, new object[0]);
               }

               oledbParameter.Value = value;
            }

            if (Command.If(out var command))
            {
               command.Parameters.Add(oledbParameter);
            }
            else
            {
               throw "Command not initialized".Throws();
            }
         }
      }

      public override void ClearAllPools() => OleDbConnection.ReleaseObjectPool();

      public override DataSource WithNewConnectionString(string newConnectionString) => new OleDbDataSource(newConnectionString, associatedFile);
   }
}