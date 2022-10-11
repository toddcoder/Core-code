using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Assertions;
using Core.Computers;
using static Core.Markup.Rtf.ParagraphFunctions;

namespace Core.Markup.Rtf;

public class BlockList : Renderable
{
   protected List<Block> blocks;
   protected CharFormat defaultCharFormat;
   protected bool allowParagraph;
   protected bool allowFootnote;
   protected bool allowControlWord;
   protected bool allowImage;
   protected bool allowTable;

   public BlockList() : this(true, true, true, true, true)
   {
   }

   public BlockList(bool allowParagraph, bool allowTable) : this(allowParagraph, true, true, true, allowTable)
   {
   }

   public BlockList(bool allowParagraph, bool allowFootnote, bool allowControlWord, bool allowImage, bool allowTable)
   {
      this.allowParagraph = allowParagraph;
      this.allowFootnote = allowFootnote;
      this.allowControlWord = allowControlWord;
      this.allowImage = allowImage;
      this.allowTable = allowTable;

      blocks = new List<Block>();
      defaultCharFormat = new CharFormat();
   }

   public CharFormat DefaultCharFormat => defaultCharFormat;

   public Paragraph Paragraph()
   {
      allowParagraph.Must().BeTrue().OrThrow("Paragraph is not allowed.");

      var block = new Paragraph(allowFootnote, allowControlWord);
      blocks.Add(block);

      return block;
   }

   public Paragraph Paragraph(string text, params object[] specifiers)
   {
      var paragraph = Paragraph();
      SetParagraphProperties(paragraph, text, specifiers);

      return paragraph;
   }

   public Section Section(SectionStartEnd type, Document doc)
   {
      var block = new Section(type, doc);
      blocks.Add(block);

      return block;
   }

   public Image Image(FileName imageFile, ImageFileType imgType)
   {
      allowImage.Must().BeTrue().OrThrow("Image is not allowed.");

      var block = new Image(imageFile, imgType);
      blocks.Add(block);

      return block;
   }

   public Image Image(FileName imageFile) => imageFile.Extension switch
   {
      ".jpg" or ".jpeg" => Image(imageFile, ImageFileType.Jpg),
      ".gif" => Image(imageFile, ImageFileType.Gif),
      ".png" => Image(imageFile, ImageFileType.Png),
      _ => throw new Exception($"Cannot determine image type from the filename extension: {imageFile}")
   };

   public Image Image(MemoryStream imageStream)
   {
      allowImage.Must().BeTrue().OrThrow("Image is not allowed.");

      var block = new Image(imageStream);
      blocks.Add(block);

      return block;
   }

   public Table Table(float horizontalWidth, float fontSize)
   {
      allowTable.Must().BeTrue().OrThrow("Table is not allowed.");

      var block = new Table(horizontalWidth, fontSize);
      blocks.Add(block);

      return block;
   }

   public void TransferBlocksTo(BlockList target)
   {
      for (var i = 0; i < blocks.Count; i++)
      {
         target.blocks.Add(blocks[i]);
      }

      blocks.Clear();
   }

   public override string Render()
   {
      var result = new StringBuilder();

      result.AppendLine();
      foreach (var block in blocks)
      {
         block.DefaultCharFormat.CopyFrom(defaultCharFormat);
         result.AppendLine(block.Render());
      }

      return result.ToString();
   }
}