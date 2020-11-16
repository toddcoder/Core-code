using System;
using System.Diagnostics;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Exceptions
{
   public static class Deep
   {
      public static string DeepMessage(this Exception exception, string format = "{level}:{message}({inner})",
         int level = 0)
      {
         if (exception.InnerException != null)
         {
            var formatter = Formatter.WithStandard(false);

            formatter["level"] = level.ToString();
            formatter["message"] = exception.Message;
            formatter["inner"] = exception.InnerException.DeepMessage(format, level + 1);

            return formatter.Format(format);
         }
         else
         {
            return exception.Message;
         }
      }

      private static IMaybe<string[]> getCallStack(StackTrace stack, string format)
      {
         if (stack.FrameCount != 0)
         {
            var stackArray = new string[stack.FrameCount];

            for (var i = 0; i < stack.FrameCount; i++)
            {
               var frameText = new Formatter();

               var frame = stack.GetFrame(i);
               frameText["line"] = frame.GetFileLineNumber().ToString();
               frameText["column"] = frame.GetFileColumnNumber().ToString();
               frameText["col"] = frame.GetFileColumnNumber().ToString();
               frameText["file"] = frame.GetFileName();

               var method = frame.GetMethod();
               frameText["method"] = method.ToNonNullString();

               frameText["class"] = method.DeclaringType?.Name ?? "";
               frameText["namespace"] = method.DeclaringType?.Namespace ?? "";

               frameText["string"] = frame.ToNonNullString();

               stackArray[i] = frameText.Format(format);
            }

            return stackArray.Some();
         }
         else
         {
            return none<string[]>();
         }
      }

      public static IMaybe<string[]> CallStack(string format = "{string}", int skip = 2)
      {
         return getCallStack(new StackTrace(skip, true), format);
      }

      public static IMaybe<string[]> CallStack(string format) => CallStack(format, 2);

      public static IMaybe<string[]> CallStack(this Exception exception, string format, int skip)
      {
         return getCallStack(new StackTrace(exception, skip, true), format);
      }

      public static IMaybe<string[]> CallStack(this Exception exception, string format)
      {
         return exception.CallStack(format, 1);
      }

      public static IMaybe<string[]> CallStack(this Exception exception) => exception.CallStack("{string}", 0);

      public static string DeepStack(this Exception exception, string format = "{level}:{stack}:\r\n{inner}",
         int level = 0)
      {
         if (exception.InnerException != null)
         {
            var formatter = Formatter.WithStandard(false);

            formatter["level"] = level.ToString();
            var stack = exception.StackTrace;
            formatter["stack"] = stack.IsNotEmpty() ? stack : "(no stack)";
            formatter["inner"] = exception.InnerException.DeepStack(format, level + 1);

            return formatter.Format(format);
         }
         else if (exception is FullStackException fse)
         {
            return fse.FullStackTrace;
         }
         else
         {
            return exception.StackTrace;
         }
      }

      public static string DeepException(this Exception exception) => $"{exception.DeepMessage()}\r\n{exception.DeepStack()}";
   }
}