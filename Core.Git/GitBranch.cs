using System;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;

namespace Core.Git
{
   public class GitBranch : IEquatable<GitBranch>
   {
      public static implicit operator GitBranch(string branch)
      {
         if (branch.Matches("^ /(-['//']+) '//' /(.+) $; f").If(out var result))
         {
            return new GitBranch(result.SecondGroup) { Origin = result.FirstGroup };
         }
         else
         {
            return new GitBranch(branch);
         }
      }

      public static GitBranch Current
      {
         get
         {
            if (Git.Execute("rev-parse --abbrev-ref HEAD").If(out var lines, out var exception))
            {
               return lines.Length > 0 ? lines[0].Trim() : throw "Branch not found".Fail();
            }
            else
            {
               throw exception;
            }
         }
      }

      protected string branch;

      public GitBranch(string branch)
      {
         this.branch = branch;

         Origin = "origin";
      }

      public string Name => branch;

      public string Origin { get; set; }

      public string FullName => $"{Origin}/{branch}";

      public Result<string[]> Delete(bool force = false)
      {
         var arguments = force ? $"-D {branch}" : $"-d {branch}";
         return Git.Execute($"branch {arguments}");
      }

      public Result<string[]> CheckOut(bool force = false)
      {
         var arguments = force ? $"{branch} --force" : branch;
         return Git.Execute($"checkout {arguments}");
      }

      public Result<GitBranch> Create(string newBranchName)
      {
         return Git.Execute($"branch -b {newBranchName}").Map(_ => (GitBranch)newBranchName);
      }

      public Result<string[]> Merge() => Git.Execute($"merge {FullName}");

      public Result<string[]> Abort() => Git.Execute("merge --abort");

      public Result<string[]> Pull() => Git.Execute("pull");

      public Result<string[]> Push(bool first = false)
      {
         var arguments = first ? $"branch --set-upstream {Origin} {branch}" : "push";
         return Git.Execute(arguments);
      }

      public Result<string[]> Reset() => Git.Execute($"reset --hard {FullName}");

      public bool IsOnRemote() => Git.Execute($"show-branch remotes/{FullName}").Map(s => s.Length > 0).Recover(_ => false);

      public Result<string[]> DifferentFromCurrent() => Git.Execute($"diff HEAD {FullName} --name-only");

      public Result<string[]> DifferentFrom(GitBranch parentBranch, bool includeStatus)
      {
         var option = includeStatus ? "name-status" : "name-only";
         return Git.Execute($"diff {parentBranch.FullName}...{FullName} --{option}");
      }

      public override string ToString() => branch;

      public bool Equals(GitBranch other) => other is not null && branch == other.branch && Origin == other.Origin;

      public override bool Equals(object obj) => obj is GitBranch other && Equals(other);

      public override int GetHashCode()
      {
         unchecked
         {
            return (branch != null ? branch.GetHashCode() : 0) * 397 ^ (Origin != null ? Origin.GetHashCode() : 0);
         }
      }

      public static bool operator ==(GitBranch left, GitBranch right) => Equals(left, right);

      public static bool operator !=(GitBranch left, GitBranch right) => !Equals(left, right);
   }
}