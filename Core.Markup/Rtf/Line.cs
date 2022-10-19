namespace Core.Markup.Rtf;

public class Line : Paragraph
{
   public override string Render() => @"\pard \brdrb \brdrs\brdrw10\brsp20 {\fs4\~}\par \pard";
}