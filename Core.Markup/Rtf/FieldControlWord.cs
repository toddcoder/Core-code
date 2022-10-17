namespace Core.Markup.Rtf;

public class FieldControlWord : Renderable
{
   protected int position;
   protected FieldType type;

   public FieldControlWord(int position, FieldType type)
   {
      this.position = position;
      this.type = type;
   }

   public int Position => position;

   public override string Render() => type switch
   {
      FieldType.Page => @"{\field{\*\fldinst PAGE }}",
      FieldType.NumPages => @"{\field{\*\fldinst NUMPAGES }}",
      FieldType.Date => @"{\field{\*\fldinst DATE }}",
      FieldType.Time => @"{\field{\*\fldinst TIME }}",
      _ => ""
   };
}