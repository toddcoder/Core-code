using System;
using System.IO;
using System.Text;
using Core.Computers;

namespace Core.Markup.Rtf;

public class Image : Block
{
   protected FileName imageFile;
   protected ImageFileType imageFileType;
   protected byte[] imageBytes;
   protected Alignment alignment;
   protected Margins margins;
   protected float width;
   protected float height;
   protected bool keepAspectRatio;
   protected string blockHead;
   protected string blockTail;
   protected bool startNewPage;
   protected bool startNewParagraph;

   public Image(FileName fileName, ImageFileType type)
   {
      imageFile = fileName;
      imageFileType = type;
      alignment = Alignment.None;
      margins = new Margins();
      keepAspectRatio = true;
      blockHead = @"{\pard";
      blockTail = @"}";
      startNewPage = false;
      startNewParagraph = false;

      var image = System.Drawing.Image.FromFile(fileName.FullPath);
      width = image.Width / image.HorizontalResolution * 72;
      height = image.Height / image.VerticalResolution * 72;

      using var mStream = new MemoryStream();
      image.Save(mStream, image.RawFormat);
      imageBytes = mStream.ToArray();
   }

   public Image(MemoryStream imageStream)
   {
      alignment = Alignment.Left;
      margins = new Margins();
      keepAspectRatio = true;
      blockHead = @"{\pard";
      blockTail = @"}";
      startNewPage = false;
      startNewParagraph = false;

      imageBytes = imageStream.ToArray();

      var image = System.Drawing.Image.FromStream(imageStream);
      width = image.Width / image.HorizontalResolution * 72;
      height = image.Height / image.VerticalResolution * 72;

      if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
      {
         imageFileType = ImageFileType.Png;
      }
      else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
      {
         imageFileType = ImageFileType.Jpg;
      }
      else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
      {
         imageFileType = ImageFileType.Gif;
      }
      else
      {
         throw new Exception($"Image format is not supported: {image.RawFormat}");
      }
   }

   public override Alignment Alignment
   {
      get => alignment;
      set => alignment = value;
   }

   public override Margins Margins => margins;

   public override bool StartNewPage
   {
      get => startNewPage;
      set => startNewPage = value;
   }

   public bool StartNewParagraph
   {
      get => startNewParagraph;
      set => startNewParagraph = value;
   }

   public float Width
   {
      get => width;
      set
      {
         if (keepAspectRatio && width > 0)
         {
            var ratio = height / width;
            height = value * ratio;
         }

         width = value;
      }
   }

   public float Height
   {
      get => height;
      set
      {
         if (keepAspectRatio && height > 0)
         {
            var ratio = width / height;
            width = value * ratio;
         }

         height = value;
      }
   }

   public bool KeepAspectRatio
   {
      get => keepAspectRatio;
      set => keepAspectRatio = value;
   }

   public override CharFormat DefaultCharFormat => new();

   protected string extractImage()
   {
      var result = new StringBuilder();

      for (var i = 0; i < imageBytes.Length; i++)
      {
         if (i != 0 && i % 60 == 0)
         {
            result.AppendLine();
         }

         result.AppendFormat("{0:x2}", imageBytes[i]);
      }

      return result.ToString();
   }

   public override string BlockHead
   {
      set => blockHead = value;
   }

   public override string BlockTail
   {
      set => blockTail = value;
   }

   public override string Render()
   {
      var result = new StringBuilder(blockHead);

      if (startNewPage)
      {
         result.Append(@"\pagebb");
      }

      if (margins[Direction.Top] >= 0)
      {
         result.Append($@"\sb{margins[Direction.Top].PointsToTwips()}");
      }

      if (margins[Direction.Bottom] >= 0)
      {
         result.Append($@"\sa{margins[Direction.Bottom].PointsToTwips()}");
      }

      if (margins[Direction.Left] >= 0)
      {
         result.Append($@"\li{margins[Direction.Left].PointsToTwips()}");
      }

      if (margins[Direction.Right] >= 0)
      {
         result.Append($@"\ri{margins[Direction.Right].PointsToTwips()}");
      }

      switch (alignment)
      {
         case Alignment.Left:
            result.Append(@"\ql");
            break;
         case Alignment.Right:
            result.Append(@"\qr");
            break;
         case Alignment.Center:
            result.Append(@"\qc");
            break;
      }

      result.AppendLine();

      result.Append(@"{\*\shppict{\pict");
      switch (imageFileType)
      {
         case ImageFileType.Jpg:
            result.Append(@"\jpegblip");
            break;
         case ImageFileType.Png or ImageFileType.Gif:
            result.Append(@"\pngblip");
            break;
         default:
            throw new Exception("Image type not supported.");
      }

      if (height > 0)
      {
         result.Append($@"\pichgoal{height.PointsToTwips()}");
      }

      if (width > 0)
      {
         result.Append($@"\picwgoal{width.PointsToTwips()}");
      }

      result.AppendLine();

      result.AppendLine(extractImage());
      result.AppendLine("}}");
      if (startNewParagraph)
      {
         result.Append(@"\par");
      }

      result.AppendLine(blockTail);
      return result.ToString();
   }
}