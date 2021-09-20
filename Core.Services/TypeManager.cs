using System;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Services.Plugins;
using Standard.Services.Plugins;
using static System.Reflection.Assembly;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using Group = Core.Configurations.Group;

namespace Core.Services
{
   public class TypeManager
   {
      protected static StringHash assemblyNamesFrom(Group assembliesGroup)
      {
         return assembliesGroup.Groups().ToStringHash(i => i.key, i => i.group.At("path"), true);
      }

      protected static StringHash typeNamesFrom(Group typesGroup)
      {
         return typesGroup.Values().ToStringHash(i => i.key, i => i.value, true);
      }

      public static Result<TypeManager> FromConfiguration(Configuration configuration)
      {
         return
            from assembliesGroup in configuration.GetGroup("assemblies").Result("There are no assemblies defined")
            let assemblyNames = assemblyNamesFrom(assembliesGroup)
            from assertedAssemblies in assemblyNames.Must().HaveCountOf(1).OrFailure()
            from typesGroup in configuration.GetGroup("types").Result("There are no types defined")
            let typeNames = typeNamesFrom(typesGroup)
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
            if (typeCache.If(name, out var type))
            {
               return type;
            }
            else if (getTypeName(name).If(out var typeName, out var exception))
            {
               if (typeName.Matches("^ -/{<} '<' -/{:} ':' /s* -/{>} '>' $; f").If(out var result))
               {
                  var (possibleTypeName, subTypeName, subAssemblyName) = result;
                  typeName = $"{possibleTypeName}`1";

                  var _result =
                     from typeFromAssembly in getTypeFromAssembly(assembly, typeName)
                     from subType in Type(subAssemblyName, subTypeName)
                     select type.MakeGenericType(subType);
                  if (_result.If(out type))
                  {
                     typeCache[name] = type;
                  }

                  return _result;
               }
               else
               {
                  var _result = getTypeFromAssembly(assembly, typeName);
                  if (_result.If(out type))
                  {
                     typeCache[name] = type;
                  }

                  return _result;
               }
            }
            else
            {
               return exception;
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
            if (assemblyCache.If(name, out var assembly))
            {
               return assembly;
            }
            else if (assemblyNames.If(name, out var path))
            {
               FolderName.Current = ((FileName)path).Folder;
               assembly = LoadFrom(path);
               assemblyCache[name] = assembly;

               return assembly;
            }
            else
            {
               return fail($"Couldn't find assembly named {name}");
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
}