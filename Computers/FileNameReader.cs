using System;
using System.IO;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Computers;

public class FileNameReader : IDisposable
{
   protected FileName file;
   protected TextReader reader;

   public FileNameReader(FileName file, bool writable)
   {
      this.file = file;

      var fileAccess = writable ? FileAccess.ReadWrite : FileAccess.Read;
      var stream = new FileStream(this.file.FullPath, FileMode.Open, fileAccess);
      reader = new StreamReader(stream);
   }

   public Optional<char> Read()
   {
      try
      {
         var result = reader.Read();
         return result > -1 ? (char)result : nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<string> Read(int index, int length)
   {
      try
      {
         var chars = new char[length];
         var count = reader.Read(chars, index, length);
         if (count > 0)
         {
            return new string(chars);
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public async Task<Completion<string>> ReadAsync(int index, int length)
   {
      try
      {
         var chars = new char[length];
         var count = await reader.ReadAsync(chars, index, length);
         if (count > 0)
         {
            return new string(chars);
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<string> ReadLine()
   {
      try
      {
         var _line = reader.ReadLine().NotNull();
         return _line.Optional();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public async Task<Completion<string>> ReadLineAsync()
   {
      try
      {
         var line = await reader.ReadLineAsync();
         return line is not null ? line : nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<string> ReadToEnd()
   {
      try
      {
         return reader.ReadToEnd();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public async Task<Completion<string>> ReadToEndAsync()
   {
      try
      {
         return await reader.ReadToEndAsync();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public void Dispose() => reader?.Dispose();
}