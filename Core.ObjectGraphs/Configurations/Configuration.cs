using Core.Applications;
using Core.Collections;
using Core.Computers;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.ObjectGraphs.Configurations
{
   public class Configuration : IHash<string, ObjectGraph>
   {
      public static IResult<Configuration> LoadFromObjectGraph(FileName file) => tryTo(() =>
      {
         FolderName.Current = file.Folder;
         var rootGraph = ObjectGraph.FromFile(file, out var replacer);
         rootGraph.Replacer = replacer;

         return new Configuration(rootGraph);
      });

      public static IResult<Configuration> FromString(string source) => tryTo(() =>
      {
         var rootGraph = ObjectGraph.FromString(source, FolderName.Current, out var replacer);
         rootGraph.Replacer = replacer;

         return new Configuration(rootGraph);
      });

      public static IResult<Configuration> FromResource<T>(string resourceName) => tryTo(() =>
      {
         var resource = new Resources<T>();
         var source = resource.String(resourceName);

         return FromString(source);
      });

      public static IResult<Configuration> LoadFromJson(FileName file) => tryTo(() => new Configuration(ObjectGraph.RootObjectGraph()));

      protected ObjectGraph rootGraph;

      public Configuration(ObjectGraph rootGraph)
      {
         this.rootGraph = rootGraph;
      }

      public ObjectGraph this[string key] => rootGraph[key];

      public bool ContainsKey(string key) => rootGraph.ContainsKey(key);

      public IResult<Hash<string, ObjectGraph>> AnyHash() => rootGraph.AnyHash();
   }
}