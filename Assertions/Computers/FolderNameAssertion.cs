using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Computers;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Computers;

public class FolderNameAssertion : IAssertion<FolderName>
{
   public static implicit operator bool(FolderNameAssertion assertion) => assertion.BeEquivalentToTrue();

   public static bool operator &(FolderNameAssertion x, ICanBeTrue y) => and(x, y);

   public static bool operator |(FolderNameAssertion x, ICanBeTrue y) => or(x, y);

   protected FolderName folder;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public FolderNameAssertion(FolderName folder)
   {
      this.folder = folder;
      constraints = new List<Constraint>();
      not = false;
      name = "Folder";
   }

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

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
      constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
      not = false;

      return this;
   }

   public FolderNameAssertion Exist() => add(() => folder.Exists(), "$name must $not exist");

   public FolderNameAssertion CreationTimeOf(DateTime dateTime)
   {
      return add(() => folder.CreationTime >= dateTime, $"$name must $not have a creation time of at least {dateTime:G}");
   }

   public FolderNameAssertion LastAccessTimeOf(DateTime dateTime)
   {
      return add(() => folder.LastAccessTime >= dateTime, $"$name must $not have a last access time of at least {dateTime:G}");
   }

   public FolderNameAssertion LastWriteTimeOf(DateTime dateTime)
   {
      return add(() => folder.LastWriteTime >= dateTime, $"$name must $not have a last write time of at least {dateTime:G}");
   }

   public FolderNameAssertion ContainFile(FileName file)
   {
      return add(() => folder.Files.Any(f => file == f), $"$name must $not contain file {file}");
   }

   public FolderNameAssertion ContainFolder(FolderName otherFolder)
   {
      return add(() => folder.Folders.Any(f => otherFolder == f), $"$name must $not contain folder {otherFolder}");
   }

   public FolderNameAssertion ChildOf(FolderName otherFolder)
   {
      var message = $"$name must $not be child of {otherFolder}";
      return add(() => folder.Parent.Map(parent => parent == otherFolder) | false, message);
   }

   public FolderNameAssertion Equal(FolderName otherFolder)
   {
      return add(() => folder == otherFolder, $"$name must $not equal {otherFolder}");
   }

   public FolderNameAssertion BeNull()
   {
      return add(() => folder == null, "$name must $not be null");
   }

   public IAssertion<FolderName> Named(string name)
   {
      this.name = name;
      return this;
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, FolderName>(this, args);

   public FolderName Force() => force(this);

   public FolderName Force(string message) => force(this, message);

   public FolderName Force(Func<string> messageFunc) => force(this, messageFunc);

   public FolderName Force<TException>(params object[] args) where TException : Exception => force<TException, FolderName>(this, args);

   public TResult Force<TResult>() => forceConvert<FolderName, TResult>(this);

   public TResult Force<TResult>(string message) => forceConvert<FolderName, TResult>(this, message);

   public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<FolderName, TResult>(this, messageFunc);

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception
   {
      return forceConvert<FolderName, TException, TResult>(this, args);
   }

   public Optional<FolderName> OrFailure() => orFailure(this);

   public Optional<FolderName> OrFailure(string message) => orFailure(this, message);

   public Optional<FolderName> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Optional<FolderName> OrNone() => orNone(this);

   public async Task<Completion<FolderName>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

   public async Task<Completion<FolderName>> OrFailureAsync(string message, CancellationToken token) =>
      await orFailureAsync(this, message, token);

   public async Task<Completion<FolderName>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}