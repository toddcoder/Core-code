using System;

namespace Core.Booleans;

public static class BooleanExtensions
{
   public static T If<T>(this bool test, Func<T> ifTrue, Func<T> ifFalse) => test ? ifTrue() : ifFalse();

   public static string Extend(this bool test, string text) => test ? text : "";

   public static string Extend(this bool test, Func<string> text) => test ? text() : "";

   public static string Extend(this bool test, string ifTrue, string ifFalse) => test ? ifTrue : ifFalse;

   public static string Extend(this bool test, Func<string> ifTrue, Func<string> ifFalse) => test ? ifTrue() : ifFalse();
}