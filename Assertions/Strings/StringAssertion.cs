using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Computers;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Strings
{
   public class StringAssertion : IAssertion<string>
   {
      public static implicit operator bool(StringAssertion assertion) => assertion.BeEquivalentToTrue();

      public static bool operator &(StringAssertion x, ICanBeTrue y) => and(x, y);

      public static bool operator |(StringAssertion x, ICanBeTrue y) => or(x, y);

      protected static bool inList(string subject, string[] strings) => strings.Any(s => subject.CompareTo(s) == 0);

      protected string subject;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public StringAssertion(string subject)
      {
         this.subject = subject;
         constraints = new List<Constraint>();
         not = false;
         name = "String";
      }

      public string Subject => subject;

      public StringAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      private static string format(string s) => $"\"{s.Elliptical(80, ' ')}\"";

      protected StringAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(Constraint.Formatted(constraintFunction, message, not, name, subject, format));
         not = false;

         return this;
      }

      public StringAssertion Equal(string obj)
      {
         return add(() => subject.CompareTo(obj) == 0, $"$name must $not equal {format(obj)}");
      }

      public StringAssertion BeGreaterThan(string obj)
      {
         return add(() => subject.CompareTo(obj) > 0, $"$name must $not be > {format(obj)}");
      }

      public StringAssertion BeGreaterThanOrEqual(string obj)
      {
         return add(() => subject.CompareTo(obj) >= 0, $"$name must $not be >= {format(obj)}");
      }

      public StringAssertion BeLessThan(string obj)
      {
         return add(() => subject.CompareTo(obj) < 0, $"$name must $not be < {format(obj)}");
      }

      public StringAssertion BeLessThanOrEqual(string obj)
      {
         return add(() => subject.CompareTo(obj) <= 0, $"$name must $not be <= {format(obj)}");
      }

      public StringAssertion BeNull()
      {
         return add(() => subject == null, "$name must $not be null");
      }

      public StringAssertion BeEmpty()
      {
         return add(() => subject == string.Empty, "$name must $not be empty");
      }

      public StringAssertion BeNullOrEmpty()
      {
         return add(() => string.IsNullOrEmpty(subject), "$name must $not be null or empty");
      }

      public StringAssertion BeNullOrWhiteSpace()
      {
         return add(() => string.IsNullOrWhiteSpace(subject), "$name must $not be null or white-space");
      }

      public StringAssertion HaveLengthOf(int length)
      {
         return add(() => subject.Length >= length, $"$name must $not have a length >= {length}");
      }

      public StringAssertion HaveLengthOfExactly(int length)
      {
         return add(() => subject.Length == length, $"$name must $not have a length of exactly {length}");
      }

      public StringAssertion BeIn(params string[] strings)
      {
         return add(() => inList(subject, strings), $"$name must $not be in {enumerableImage(strings)}");
      }

      public StringAssertion StartWith(string substring)
      {
         return add(() => subject.StartsWith(substring), $"$name must start with {format(substring)}");
      }

      public StringAssertion EndWith(string substring)
      {
         return add(() => subject.EndsWith(substring), $"$name must end with {format(substring)}");
      }

      public StringAssertion Match(string pattern)
      {
         return add(() => subject.IsMatch(pattern), $"$name must $not match regex {format(pattern)}");
      }

      public StringAssertion BeAValidFileName()
      {
         return add(() => FileName.IsValidFileName(subject), "$name must $not be a valid file name");
      }

      public StringAssertion BeAValidFolderName()
      {
         return add(() => FolderName.IsValidFolderName(subject), "$name must $not be a valid folder name");
      }

      public string Value => subject;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public IAssertion<string> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, string>(this, args);

      public string Force() => force(this);

      public string Force(string message) => force(this, message);

      public string Force(Func<string> messageFunc) => force(this, messageFunc);

      public string Force<TException>(params object[] args) where TException : Exception => force<TException, string>(this, args);

      public TResult Force<TResult>() => throw "Can't convert string to another type".Throws();

      public TResult Force<TResult>(string message) => Force<TResult>();

      public TResult Force<TResult>(Func<string> messageFunc) => Force<TResult>();

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception => Force<TResult>();

      public Result<string> OrFailure() => orFailure(this);

      public Result<string> OrFailure(string message) => orFailure(this, message);

      public Result<string> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public Maybe<string> OrNone() => orNone(this);

      public async Task<ICompletion<string>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<string>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<string>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}