using System;
using Core.Exceptions;

namespace Core.Assertion
{
   internal static class AssertionFunctions
   {
      internal static bool isNot(bool test, bool not) => test && !not;
   }
}