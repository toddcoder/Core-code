using Core.Monads;
using Core.Strings;

namespace Core.Git;

public class GitTag
{
   public static implicit operator GitTag(string tag) => new(tag);

   public static Optional<string[]> List(string pattern = "")
   {
      var arguments = pattern.IsNotEmpty() ? "tag" : $"tag -l \"{pattern}\"";
      return Git.TryTo.Execute(arguments);
   }

   protected string tag;

   public GitTag(string tag)
   {
      this.tag = tag;

      Origin = "origin";
   }

   public string Origin { get; set; }

   public Optional<string[]> Create() => Git.TryTo.Execute($"tag {tag}");

   public Optional<string[]> Push() => Git.TryTo.Execute($"push {Origin} --tags");
}