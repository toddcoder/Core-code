using System;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using Standard.Services;
using static System.Reflection.Assembly;
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

      public Type Type(string assemblyName, string typeName)
      {
         if (typeName == "seq")
         {
            return typeof(Sequence);
         }
         else if (assemblyName == "$")
         {
            typeName = getTypeName(typeName);
            return System.Type.GetType(typeName);
         }
         else
         {
            var assembly = getAssemblyFromCache(assemblyName);
            return getTypeFromCache(typeName, assembly);
         }
      }

      Type getTypeFromCache(string name, Assembly assembly) => typeCache.Find(name, n =>
      {
         var typeName = getTypeName(name);
         var match = typeName.Matches("^ -/{<} '<' -/{:} ':' /s* -/{>} '>' $");
         if (match.If(out var m))
         {
            var (tn, subTypeName, subAssemblyName) = m;
            typeName = $"{tn}`1";
            var type = getTypeFromAssembly(assembly, typeName);
            var subType = Type(subAssemblyName, subTypeName);
            return type.MakeGenericType(subType);
         }
         else
         {
            return getTypeFromAssembly(assembly, typeName);
         }
      }, true);

      string getTypeName(string name) => typeNames.Value(name);

      static Type getTypeFromAssembly(Assembly assembly, string typeName) => assembly.GetType(typeName, true);

      Assembly getAssemblyFromCache(string name) => assemblyCache.Find(name, n =>
      {
         if (assemblyNames.If(name, out var path))
         {
            FolderName.Current = ((FileName)path).Folder;
            return LoadFrom(path);
         }
         else
         {
            throw $"Couldn't find assembly named {name}".Throws();
         }
      }, true);

      public IMaybe<string> DefaultAssemblyName => _defaultAssemblyName;

      public IMaybe<string> DefaultTypeName => _defaultTypeName;

      public TypeManagerTrying TryTo => new TypeManagerTrying(this);
   }
}