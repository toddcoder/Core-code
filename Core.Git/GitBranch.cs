﻿using System;
using System.Collections.Generic;
using Core.Matching;
using static Core.Monads.MonadFunctions;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Git;

public class GitBranch : IEquatable<GitBranch>
{
   public static implicit operator GitBranch(string branch)
   {
      var _result = branch.Matches("^ /(-['//']+) '//' /(.+) $; f");
      if (_result is (true, var (origin, branchName)))
      {
         return new GitBranch(branchName) { Origin = origin };
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
         var _lines = Git.TryTo.Execute("rev-parse --abbrev-ref HEAD");
         if (_lines is (true, var lines))
         {
            return lines.Length > 0 ? lines[0].Trim() : throw fail("Branch not found");
         }
         else
         {
            throw _lines.Exception;
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

   public GitBranchTry TryTo => new(this);

   public IEnumerable<GitResult> Delete(bool force = false)
   {
      var arguments = force ? $"-D {branch}" : $"-d {branch}";
      return Git.Execute($"branch {arguments}");
   }

   public IEnumerable<GitResult> CheckOut(bool force = false)
   {
      var arguments = force ? $"{branch} --force" : branch;
      return Git.Execute($"checkout {arguments}");
   }

   public IEnumerable<GitResult> Create(string newBranchName) => Git.Execute($"branch -b {newBranchName}");

   public IEnumerable<GitResult> Merge() => Git.Execute($"merge {FullName}");

   public IEnumerable<GitResult> Abort() => Git.Execute("merge --abort");

   public IEnumerable<GitResult> Pull() => Git.Execute("pull");

   public IEnumerable<GitResult> Push(bool first = false)
   {
      var arguments = first ? $"branch --set-upstream {Origin} {branch}" : "push";
      return Git.Execute(arguments);
   }

   public IEnumerable<GitResult> Reset() => Git.Execute($"reset --hard {FullName}");

   public bool IsOnRemote() => Git.TryTo.Execute($"show-branch remotes/{FullName}").Map(s => s.Length > 0) | false;

   public IEnumerable<GitResult> DifferentFromCurrent() => Git.Execute($"diff HEAD {FullName} --name-only");

   public IEnumerable<GitResult> DifferentFrom(GitBranch parentBranch, bool includeStatus)
   {
      var option = includeStatus ? "name-status" : "name-only";
      return Git.Execute($"diff {parentBranch.FullName}...{FullName} --{option}");
   }

   public override string ToString() => branch;

   public bool Equals(GitBranch other) => other is not null && branch == other.branch && Origin == other.Origin;

   public override bool Equals(object obj) => obj is GitBranch other && Equals(other);

   public override int GetHashCode() => hashCode() + branch + Origin;

   public static bool operator ==(GitBranch left, GitBranch right) => Equals(left, right);

   public static bool operator !=(GitBranch left, GitBranch right) => !Equals(left, right);
}