using System;
using Core.Computers;

namespace Core.Applications.Writers;

public class FileWriter : BaseWriter, IDisposable
{
   protected FileName file;

   public FileWriter(FileName file, bool useBuffer)
   {
      this.file = file;
      this.file.UseBuffer = useBuffer;
   }

   protected override void writeRaw(string text) => file.Append(text);

   protected void dispose() => file.Flush();

   public void Dispose()
   {
      dispose();
      GC.SuppressFinalize(this);
   }

   ~FileWriter() => dispose();
}