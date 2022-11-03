using System;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using static System.Reflection.Assembly;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Services;

public class TypeManager
{
   protected static StringHash assemblyNamesFrom(Setting assembliesSetting)
   {
      return assembliesSetting.Settings().ToStringHash(i => i.key, i => i.setting.Value.String("path"), true);
   }

   protected static StringHash typeNamesFrom(Setting typesSetting)
   {
      return typesSetting.Items().ToStringHash(i => i.key, i => i.text, true);
   }

   public static Result<TypeManager> FromConfiguration(Configuration configuration)
   {
      return
         from assembliesSetting in configuration.Result.Setting("assemblies")
         let assemblyNames = assemblyNamesFrom(assembliesSetting)
         from assertedAssemblies in assemblyNames.Must().HaveCountOf(1).OrFailure()
         from typesSetting in configuration.Result.Setting("types")
         let typeNames = typeNamesFrom(typesSetting)
         from assertedTypes in typeNames.Must().HaveCountOf(1).OrFailure()
         select new TypeManager(assemblyNames, typeNames);
   }

   protected StringHash<Assembly> assemblyCache;
   protected StringHash assemblyNames;
   protected StringHash<Type> typeCache;
   protected StringHash typeNames;
   protected Maybe<string> _defaultAssemblyName;
   protected Maybe<string> _defaultTypeName;

   public TypeManager(StringHash assemblyNames, StringHash typeNames)
   {
      this.assemblyNames = assemblyNames;
      this.typeNames = typeNames;

      assemblyCache = new StringHash<Assembly>(true);
      _defaultAssemblyName = assemblyNames.Tuples().FirstOrNone(i => i.key == "default").Map((_, path) => path);

      typeCache = new StringHash<Type>(true);
      _defaultTypeName = typeNames.Tuples().FirstOrNone(i => i.key == "default").Map((_, typeName) => typeName);
   }

   public Result<Type> Type(string assemblyName, string typeName)
   {
      if (assemblyName == "$")
      {
         return getTypeName(typeName).Map(System.Type.GetType);
      }
      else
      {
         return
            from assembly in getAssemblyFromCache(assemblyName)
            from type in getTypeFromCache(typeName, assembly)
            select type;
      }
   }

   protected Result<Type> getTypeFromCache(string name, Assembly assembly)
   {
      try
      {
         var _type = typeCache.Of(name);
         if (_type)
         {
            return ~_type;
         }
         else
         {
            var _typeName = getTypeName(name);
            if (_typeName)
            {
               var typeName = ~_typeName;
               var _result = typeName.Matches("^ -/{<} '<' -/{:} ':' /s* -/{>} '>' $; f");
               if (_result)
               {
                  var (possibleTypeName, subTypeName, subAssemblyName) = ~_result;
                  typeName = $"{possibleTypeName}`1";

                  var _genericType =
                     from typeFromAssembly in getTypeFromAssembly(assembly, typeName)
                     from subType in Type(subAssemblyName, subTypeName)
                     select (~_type).MakeGenericType(subType);
                  if (_genericType)
                  {
                     typeCache[name] = _genericType;
                  }

                  return _genericType;
               }
               else
               {
                  var _assemblyType = getTypeFromAssembly(assembly, typeName);
                  if (_assemblyType)
                  {
                     typeCache[name] = _assemblyType;
                  }

                  return _assemblyType;
               }
            }
            else
            {
               return _typeName.Exception;
            }
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Result<string> getTypeName(string name) => typeNames.Map(name).Result($"Couldn't determine type name {name}");

   protected static Result<Type> getTypeFromAssembly(Assembly assembly, string typeName) => tryTo(() => assembly.GetType(typeName, true));

   protected Result<Assembly> getAssemblyFromCache(string name)
   {
      try
      {
         var _assembly = assemblyCache.Maybe[name];
         if (_assembly)
         {
            return ~_assembly;
         }
         else
         {
            var _path = assemblyNames.Maybe[name];
            if (_path)
            {
               FolderName.Current = ((FileName)~_path).Folder;
               var assembly = LoadFrom(_path);
               assemblyCache[name] = assembly;

               return assembly;
            }
            else
            {
               return fail($"Couldn't find assembly named {name}");
            }
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Maybe<string> DefaultAssemblyName => _defaultAssemblyName;

   public Maybe<string> DefaultTypeName => _defaultTypeName;
}