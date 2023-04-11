using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.Computers;

public class FileNameWriter : IDisposable
{
   protected FileName file;
   protected TextWriter writer;

   public FileNameWriter(FileName file, bool create, bool @new, bool append, bool truncate, bool readable)
   {
      this.file = file;

      FileMode fileMode;
      if (append)
      {
         fileMode = FileMode.Append;
      }
      else if (truncate)
      {
         fileMode = FileMode.Truncate;
      }
      else if (create)
      {
         if (@new)
         {
            fileMode = FileMode.Create;
         }
         else
         {
            fileMode = FileMode.OpenOrCreate;
         }
      }
      else
      {
         fileMode = FileMode.Open;
      }

      var fileAccess = readable ? FileAccess.ReadWrite : FileAccess.Write;
      var stream = new FileStream(this.file.FullPath, fileMode, fileAccess);
      writer = new StreamWriter(stream);
   }

   protected static string getString(object value) => value?.ToString() ?? "";

   public void Write(object value) => writer.Write(getString(value));

   public async Task WriteAsync(object value) => await writer.WriteAsync(getString(value));

   public void WriteLine(object value) => writer.WriteLine(getString(value));

   public async Task WriteLineAsync(object value)=> await writer.WriteLineAsync(getString(value));

   public void Dispose() => writer?.Dispose();
}