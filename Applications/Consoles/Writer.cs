using System;
using Core.Strings;

namespace Core.Applications.Consoles;

public abstract class Writer
{
   public abstract void WriteRaw(string text);

   public abstract string EndOfLine { get; }

   public abstract string FormatException(Exception exception);

   public virtual Writer Write(object obj)
   {
      WriteRaw(obj.ToNonNullString());
      return this;
   }

   public virtual Writer WriteLine(object obj)
   {
      Write(obj);
      Write(EndOfLine);

      return this;
   }

   public virtual Writer Write(Exception exception)
   {
      WriteLine(FormatException(exception));

      return this;
   }

   public virtual Writer At(float x, float y)
   {
      return this;
   }

   public virtual Writer ForeColor(string colorName)
   {
      return this;
   }

   public virtual Writer BackColor(string colorName)
   {
      return this;
   }

   public virtual Writer Clear()
   {
      return this;
   }

   public virtual Writer Clear(string colorName)
   {
      return this;
   }

   public float X => 0;

   public float Y => 0;

   public virtual Writer Window(float x, float y, float with, float height)
   {
      return this;
   }
}