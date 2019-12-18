using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Computers;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Computers
{
   public class FolderNameAssertion : IAssertion<FolderName>
   {
      public static implicit operator bool(FolderNameAssertion assertion) => assertion.BeTrue();

      public static bool operator &(FolderNameAssertion x, ICanBeTrue y) => and(x, y);

      public static bool operator |(FolderNameAssertion x, ICanBeTrue y) => or(x, y);

      protected FolderName folder;
      protected List<Constraint> constraints;
      protected bool not;

      public FolderNameAssertion(FolderName folder)
      {
         this.folder = folder;
         constraints = new List<Constraint>();
         not = false;
      }

      public bool BeTrue() => beTrue(this);

      public FolderName Value => folder;

      public IEnumerable<Constraint> Constraints => constraints;

      public FolderNameAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected FolderNameAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public FolderNameAssertion Exist() => add(() => folder.Exists(), $"{folder} must $not exist");

      public FolderNameAssertion CreationTimeOf(DateTime dateTime)
      {
         return add(() => folder.CreationTime >= dateTime, $"{folder} must $not have a creation time of at least {dateTime:G}");
      }

      public FolderNameAssertion LastAccessTimeOf(DateTime dateTime)
      {
         return add(() => folder.LastAccessTime >= dateTime, $"{folder} must $not have a last access time of at least {dateTime:G}");
      }

      public FolderNameAssertion LastWriteTimeOf(DateTime dateTime)
      {
         return add(() => folder.LastWriteTime >= dateTime, $"{folder} must $not have a last write time of at least {dateTime:G}");
      }

      public FolderNameAssertion ContainFile(FileName file)
      {
         return add(() => folder.Files.Any(f => file == f), $"{folder} must $not contain file {file}");
      }

      public FolderNameAssertion ContainFolder(FolderName otherFolder)
      {
         return add(() => folder.Folders.Any(f => otherFolder == f), $"{folder} must $not contain folder {otherFolder}");
      }

      public FolderNameAssertion ChildOf(FolderName otherFolder)
      {
         var message = $"{folder} must $not be child of {otherFolder}";
         return add(() => folder.Parent.Map(parent => parent == otherFolder).DefaultTo(() => false), message);
      }

      public FolderNameAssertion Equal(FolderName otherFolder)
      {
         return add(() => folder == otherFolder, $"{folder} must $not equal {otherFolder}");
      }

      public FolderNameAssertion BeNull()
      {
         return add(() => folder != null, "folder must $not be null");
      }

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, FolderName>(this, args);

      public FolderName Ensure() => ensure(this);

      public FolderName Ensure(string message) => ensure(this, message);

      public FolderName Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public FolderName Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, FolderName>(this, args);

      public TResult Ensure<TResult>() => ensureConvert<FolderName, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<FolderName, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<FolderName, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<FolderName, TException, TResult>(this, args);
      }

      public IResult<FolderName> Try() => @try(this);

      public IResult<FolderName> Try(string message) => @try(this, message);

      public IResult<FolderName> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public IMaybe<FolderName> Maybe() => maybe(this);

      public async Task<ICompletion<FolderName>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<FolderName>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<FolderName>> TryAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await tryAsync(this, messageFunc, token);
      }
   }
}