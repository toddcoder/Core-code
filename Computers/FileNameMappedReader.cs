using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Core.Computers;

public class FileNameMappedReader : TextReader
{
   protected FileStream fileStream;
   protected MemoryMappedFile mappedFile;
   protected MemoryMappedViewStream viewStream;
   protected StreamReader reader;

   public FileNameMappedReader(FileName file)
   {
      fileStream = new FileStream(file.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      mappedFile = MemoryMappedFile.CreateFromFile(fileStream, null, 0, MemoryMappedFileAccess.Read, null,
         HandleInheritability.None, false);
      viewStream = mappedFile.CreateViewStream(0, file.Length, MemoryMappedFileAccess.Read);
      reader = new StreamReader(viewStream);
   }

   public FileNameMappedReader(FileName file, Encoding encoding)
   {
      fileStream = new FileStream(file.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      mappedFile = MemoryMappedFile.CreateFromFile(fileStream, null, 0, MemoryMappedFileAccess.Read, null,
         HandleInheritability.None, false);
      viewStream = mappedFile.CreateViewStream(0, file.Length, MemoryMappedFileAccess.Read);
      reader = new StreamReader(viewStream, encoding);
   }

   public override void Close()
   {
      base.Close();

      reader?.Dispose();
      viewStream?.Dispose();
      mappedFile?.Dispose();
      fileStream?.Dispose();
   }

   public override int Peek() => reader.Peek();

   public override int Read() => reader.Read();

   public bool EndOfStream => reader.EndOfStream;

   protected override void Dispose(bool disposing)
   {
      base.Dispose(disposing);

      if (disposing)
      {
         reader?.Dispose();
         viewStream?.Dispose();
         mappedFile?.Dispose();
         fileStream?.Dispose();
      }
   }
}