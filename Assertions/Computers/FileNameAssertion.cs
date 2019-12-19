using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Computers;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Computers
{
   public class FileNameAssertion : IAssertion<FileName>
   {
      public static implicit operator bool(FileNameAssertion assertion) => assertion.BeTrue();

      public static bool operator &(FileNameAssertion x, ICanBeTrue y) => and(x, y);

      public static bool operator |(FileNameAssertion x, ICanBeTrue y) => or(x, y);

      protected FileName file;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public FileNameAssertion(FileName file)
      {
         this.file = file;
         constraints = new List<Constraint>();
         not = false;
         name = "File";
      }

      public bool BeTrue() => beTrue(this);

      public FileName Value => file;

      public IEnumerable<Constraint> Constraints => constraints;

      public FileNameAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      protected FileNameAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name));
         not = false;

         return this;
      }

      public FileNameAssertion Exist() => add(() => file.Exists(), "$name must $not exist");

      public FileNameAssertion HaveLengthOf(long length) => add(() => file.Length >= length, $"$name must $not have a length of at least {length}");

      public FileNameAssertion CreationTimeOf(DateTime dateTime)
      {
         return add(() => file.CreationTime >= dateTime, $"$name must $not have a creation time of at least {dateTime:G}");
      }

      public FileNameAssertion LastAccessTimeOf(DateTime dateTime)
      {
         return add(() => file.LastAccessTime >= dateTime, $"$name must $not have a last access time of at least {dateTime:G}");
      }

      public FileNameAssertion LastWriteTimeOf(DateTime dateTime)
      {
         return add(() => file.LastWriteTime >= dateTime, $"$name must $not have a last write time of at least {dateTime:G}");
      }

      public FileNameAssertion HaveExtensionOf(string extension)
      {
         return add(() => file.Extension == extension, $"$name must $not have an extension of {extension}");
      }

      public FileNameAssertion HaveSameExtensionAs(FileName otherFile)
      {
         return add(() => file.Extension == otherFile.Extension, $"$name must $not have same extension as {otherFile}");
      }

      public FileNameAssertion BeInFolder(FolderName folder)
      {
         return add(() => file.Folder == folder, $"$name must $not be in folder {folder}");
      }

      public FileNameAssertion HaveNameOf(string name)
      {
         return add(() => file.Name == name, "$name must $not have name $name");
      }

      public FileNameAssertion HaveSameNameAs(FileName otherFile)
      {
         return add(() => file.Name == otherFile.Name, $"$name must $not have same name as {otherFile}");
      }

      public FileNameAssertion HaveNameExtensionOf(string nameExtension)
      {
         return add(() => file.NameExtension == nameExtension, $"$name must $not have name + extension {nameExtension}");
      }

      public FileNameAssertion HaveSameNameExtensionAs(FileName otherFile)
      {
         return add(() => file.NameExtension == otherFile.NameExtension, $"$name must $not have same name + extension as {otherFile}");
      }

      public FileNameAssertion Equal(FileName otherFile)
      {
         return add(() => file == otherFile, $"$name must $not equal {otherFile}");
      }

      public FileNameAssertion BeNull()
      {
         return add(() => file == null, "$name must $not be null");
      }

      public IAssertion<FileName> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, FileName>(this, args);

      public FileName Ensure() => ensure(this);

      public FileName Ensure(string message) => ensure(this, message);

      public FileName Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public FileName Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, FileName>(this, args);

      public TResult Ensure<TResult>() => ensureConvert<FileName, TResult>(this);

      public TResult Ensure<TResult>(string message) => ensureConvert<FileName, TResult>(this, message);

      public TResult Ensure<TResult>(Func<string> messageFunc) => ensureConvert<FileName, TResult>(this, messageFunc);

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
      {
         return ensureConvert<FileName, TException, TResult>(this, args);
      }

      public IResult<FileName> Try() => @try(this);

      public IResult<FileName> Try(string message) => @try(this, message);

      public IResult<FileName> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public IMaybe<FileName> Maybe() => maybe(this);

      public async Task<ICompletion<FileName>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<FileName>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<FileName>> TryAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await tryAsync(this, messageFunc, token);
      }
   }
}