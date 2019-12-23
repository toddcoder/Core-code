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

      static string format(FileName file) => file.TruncateBySubfolder(1);

      protected FileNameAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(Constraint.Formatted(constraintFunction, message, not, name, Value, format));
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
         return add(() => file.Extension == otherFile.Extension, $"$name must $not have same extension as {format(otherFile)}");
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
         return add(() => file.Name == otherFile.Name, $"$name must $not have same name as {format(otherFile)}");
      }

      public FileNameAssertion HaveNameExtensionOf(string nameExtension)
      {
         return add(() => file.NameExtension == nameExtension, $"$name must $not have name + extension {nameExtension}");
      }

      public FileNameAssertion HaveSameNameExtensionAs(FileName otherFile)
      {
         return add(() => file.NameExtension == otherFile.NameExtension, $"$name must $not have same name + extension as {format(otherFile)}");
      }

      public FileNameAssertion Equal(FileName otherFile)
      {
         return add(() => file == otherFile, $"$name must $not equal {format(otherFile)}");
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

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, FileName>(this, args);

      public FileName Force() => force(this);

      public FileName Force(string message) => force(this, message);

      public FileName Force(Func<string> messageFunc) => force(this, messageFunc);

      public FileName Force<TException>(params object[] args) where TException : Exception => force<TException, FileName>(this, args);

      public TResult Force<TResult>() => forceConvert<FileName, TResult>(this);

      public TResult Force<TResult>(string message) => forceConvert<FileName, TResult>(this, message);

      public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<FileName, TResult>(this, messageFunc);

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception
      {
         return forceConvert<FileName, TException, TResult>(this, args);
      }

      public IResult<FileName> OrFailure() => orFailure(this);

      public IResult<FileName> OrFailure(string message) => orFailure(this, message);

      public IResult<FileName> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public IMaybe<FileName> OrNone() => orNone(this);

      public async Task<ICompletion<FileName>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<FileName>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<FileName>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }
   }
}