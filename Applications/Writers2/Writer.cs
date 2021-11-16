using System;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Writers2
{
   public abstract class Writer
   {
      protected string lineEnding;
      protected bool deepMessage;
      protected Maybe<string> _newForeColor;
      protected Maybe<string> _newBackColor;

      public Writer()
      {
         lineEnding = "\r\n";
         deepMessage = true;
         _newForeColor = nil;
         _newBackColor = nil;
      }

      public abstract void WriteRaw(string text);

      public abstract void SetColors(string foreColor, string backColor);

      public abstract void SetLocation(int left, int top);

      public abstract string DefaultForeColor { get; }

      public abstract string DefaultBackColor { get; }

      public abstract string DefaultExceptionForeColor { get; }

      public abstract string DefaultExceptionBackColor { get; }

      public abstract int CurrentLeft { get; }

      public abstract int CurrentTop { get; }

      public virtual string ForeColor
      {
         get => _newForeColor.DefaultTo(() => DefaultForeColor);
         set => _newForeColor = value;
      }

      public virtual string BackColor
      {
         get => _newBackColor.DefaultTo(() => DefaultBackColor);
         set => _newBackColor = value;
      }

      public virtual string LineEnding
      {
         get => lineEnding;
         set => lineEnding = value;
      }

      public virtual bool DeepMessage
      {
         get => deepMessage;
         set => deepMessage = value;
      }

      protected void setColors()
      {
         var foreColor = _newForeColor.DefaultTo(() => DefaultForeColor);
         _newForeColor = nil;
         var backColor = _newBackColor.DefaultTo(() => DefaultBackColor);
         _newBackColor = nil;

         SetColors(foreColor, backColor);
      }

      protected void setExceptionColors()
      {
         var foreColor = _newForeColor.DefaultTo(() => DefaultExceptionForeColor);
         _newForeColor = nil;
         var backColor = _newBackColor.DefaultTo(() => DefaultExceptionBackColor);
         _newBackColor = nil;

         SetColors(foreColor, backColor);
      }

      protected virtual void writeRaw(string message)
      {
         setColors();
         WriteRaw(message);
      }

      protected virtual void writeExceptionRaw(string message)
      {
         setExceptionColors();
         WriteRaw(message);
      }

      public virtual void Write(string message) => writeRaw(message);

      public virtual void Write(object message) => Write(message.ToNonNullString());

      public virtual void WriteLine(string message) => Write(message + lineEnding);

      public virtual void WriteLine(object message) => WriteLine(message.ToNonNullString());

      public virtual void WriteException(Exception exception)
      {
         setExceptionColors();
         WriteRaw(deepMessage ? exception.DeepMessage() : exception.Message);
      }

      public virtual void WriteExceptionLine(Exception exception)
      {
         WriteLine(deepMessage ? exception.DeepMessage() : exception.Message);
      }

      public virtual void WriteException(string message) => writeExceptionRaw(message);

      public virtual void WriteException(object message) => WriteException(message.ToNonNullString());

      public virtual void WriteExceptionLine(string message) => WriteException(message + lineEnding);

      public virtual void WriteExceptionLine(object message) => WriteExceptionLine(message.ToNonNullString() + lineEnding);

      public Writer At(int left, int top)
      {
         SetLocation(left, top);
         return this;
      }
   }
}