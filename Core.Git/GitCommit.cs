using System;
using Core.Assertions;
using Core.Monads;

namespace Core.Git;

public class GitCommit : IEquatable<GitCommit>
{
   public static implicit operator GitCommit(string hash)
   {
      hash.Must().Match("^ ['a-f0-9']40 | ['a-f0-9']12 $; f").OrThrow("Hash must be a 40 or 12 hexadecimal string");
      return new GitCommit(hash);
   }

   protected string hash;

   public GitCommit(string hash)
   {
      this.hash = hash;
   }

   public Optional<string[]> ShowMerge() => Git.TryTo.Execute($"show --oneline --no-patch -m {hash}");

   public bool Equals(GitCommit other) => hash is not null && hash == other.hash;

   public override bool Equals(object obj) => obj is GitCommit other && Equals(other);

   public override int GetHashCode() => hash != null ? hash.GetHashCode() : 0;

   public static bool operator ==(GitCommit left, GitCommit right) => Equals(left, right);

   public static bool operator !=(GitCommit left, GitCommit right) => !Equals(left, right);
}