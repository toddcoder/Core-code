using System.IO;

namespace Core.Computers
{
   public class FileNameWriter : StringWriter
   {
      protected FileName file;

      public FileNameWriter(FileName file)
      {
         this.file = file;
      }

      public void ToFile() => file.Text = ToString();
   }
}