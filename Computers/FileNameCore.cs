using System.IO;

namespace Core.Computers;

public class FileNameCore
{
   protected FileName file;

   public FileNameCore(FileName file)
   {
      this.file = file;
   }

   public string Text
   {
      get
      {
         using var stream = new FileStream(file.ToString(), FileMode.Open, FileAccess.Read);
         using var reader = new StreamReader(stream);

         return reader.ReadToEnd();
      }
   }
}