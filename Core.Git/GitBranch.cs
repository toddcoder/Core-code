using Core.Exceptions;
using Core.Monads;

namespace Core.Git
{
   public class GitBranch
   {
      public static implicit operator GitBranch(string branch) => new(branch);

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

      public string Origin { get; set; }

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

      public Result<string[]> Merge() => Git.Execute($"merge {Origin}/{branch}");

      public Result<string[]> Pull() => Git.Execute("pull");

      public Result<string[]> Push(bool first = false)
      {
         var arguments = first ? $"branch --set-upstream {Origin} {branch}" : "push";
         return Git.Execute(arguments);
      }
   }
}