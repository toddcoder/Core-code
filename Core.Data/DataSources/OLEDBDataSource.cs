using System;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using Core.Computers;
using Core.Dates;
using Core.Exceptions;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static System.Convert;
using static Core.Monads.MonadFunctions;

namespace Core.Data.DataSources
{
   public class OLEDBDataSource : DataSource
   {
      static OleDbType typeToOLEDBType(Type type)
      {
         switch (Type.GetTypeCode(type))
         {
            case TypeCode.Boolean:
               return OleDbType.Boolean;
            case TypeCode.Byte:
               return OleDbType.UnsignedTinyInt;
            case TypeCode.Char:
               return OleDbType.Char;
            case TypeCode.DateTime:
               return OleDbType.DBTimeStamp;
            case TypeCode.Decimal:
               return OleDbType.Decimal;
            case TypeCode.Double:
               return OleDbType.Double;
            case TypeCode.Int16:
               return OleDbType.SmallInt;
            case TypeCode.Int32:
               return OleDbType.Integer;
            case TypeCode.Int64:
               return OleDbType.BigInt;
            case TypeCode.Object:
               return OleDbType.Variant;
            case TypeCode.String:
               return OleDbType.VarWChar;
            default:
               throw $"Doesn't support {type}".Throws();
         }
      }

      protected IMaybe<FileName> associatedFile;

      public OLEDBDataSource(string connectionString, IMaybe<FileName> associatedFile) : base(connectionString,
         "30 seconds".ToTimeSpan())
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
            if (parameter.Type.If(out var parameterType)) { }
            else
            {
               parameter.DeterminePropertyType(entity);
               parameterType = parameter.PropertyType;
               parameter.Type = parameterType.Some();
            }

            OleDbParameter oledbParameter;
            if (parameter.Size.If(out var size))
            {
               oledbParameter = new OleDbParameter(parameter.Name, typeToOLEDBType(parameterType), size);
            }
            else
            {
               oledbParameter = new OleDbParameter(parameter.Name, typeToDBType(parameterType));
            }

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
               if (value.IsNull() && parameter.Default.If(out var defaultValue))
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

      public override DataSource WithNewConnectionString(string newConnectionString) => new OLEDBDataSource(newConnectionString, associatedFile);
   }
}