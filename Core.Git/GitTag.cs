using Core.Monads;
using Core.Strings;

namespace Core.Git
{
   public class GitTag
   {
      public static implicit operator GitTag(string tag) => new(tag);

      public static Result<string[]> List(string pattern = "")
      {
         var arguments = pattern.IsNotEmpty() ? "tag" : $"tag -l \"{pattern}\"";
         return Git.Execute(arguments);
      }

      protected string tag;

      public GitTag(string tag)
      {
         this.tag = tag;

         Origin = "origin";
      }

      public string Origin { get; set; }

      public Result<string[]> Create() => Git.Execute($"tag {tag}");

      public Result<string[]> Push() => Git.Execute($"push {Origin} --tags");
   }
}