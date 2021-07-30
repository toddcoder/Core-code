using Core.Collections;

namespace Core.Markup.Rtf
{
   public class FieldControlWord : Renderable
   {
      public enum FieldType
      {
         None = 0,
         Page,
         NumPages,
         Date,
         Time,
      }

      protected static AutoHash<FieldType, string> controlWords;

      static FieldControlWord()
      {
         controlWords = new AutoHash<FieldType, string>(string.Empty)
         {
            [FieldType.None] = string.Empty,
            [FieldType.Page] = @"{\field{\*\fldinst PAGE }}",
            [FieldType.NumPages] = @"{\field{\*\fldinst NUMPAGES }}",
            [FieldType.Date] = @"{\field{\*\fldinst DATE }}",
            [FieldType.Time] = @"{\field{\*\fldinst TIME }}"
         };
      }

      protected int position;
      protected FieldType type;

      public FieldControlWord(int position, FieldType type)
      {
         this.position = position;
         this.type = type;
      }

      public int Position => position;

      public override string Render() => controlWords[type];
   }
}