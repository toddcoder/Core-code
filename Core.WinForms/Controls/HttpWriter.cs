using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using Core.Dates.DateIncrements;
using Core.Strings;

namespace Core.WinForms.Controls;

public class HttpWriter
{
   protected static HttpClient httpClient;

   static HttpWriter()
   {
      var handler = new HttpClientHandler { Credentials = CredentialCache.DefaultCredentials };
      httpClient = new HttpClient(handler) { Timeout = 10.Seconds() };
   }

   protected string url;
   protected Rectangle rectangle;
   protected Font font;
   protected Rectangle dashedRectangle;
   protected Rectangle fillRectangle;

   public HttpWriter(string url, Rectangle rectangle, Font font)
   {
      this.url = url;
      this.rectangle = rectangle;
      this.font = font;

      dashedRectangle = this.rectangle;
      dashedRectangle.Inflate(-10, -10);

      fillRectangle = dashedRectangle;
      fillRectangle.Inflate(-2, -2);
   }

   public static bool IsGoodUrl(string url)
   {
      if (url.IsEmpty())
      {
         return false;
      }

      var response = httpClient.GetAsync(url).Result;
      return response.StatusCode == HttpStatusCode.OK;
   }

   protected static Color getForeColor(bool wasGood) => wasGood ? Color.White : Color.Black;

   protected static Color getBackColor(bool wasGood) => wasGood ? Color.Green : Color.Gold;

   public void OnPaintBackground(Graphics graphics, bool wasGood, bool mouseOver)
   {
      var foreColor = getForeColor(wasGood);
      var backColor = getBackColor(wasGood);

      using var brush = new SolidBrush(backColor);
      graphics.FillRectangle(brush, rectangle);

      if (mouseOver)
      {
         using var pen = new Pen(foreColor);
         pen.DashStyle = DashStyle.Dash;
         graphics.DrawRectangle(pen, dashedRectangle);
      }

      using var fillBrush = new SolidBrush(foreColor);
      graphics.FillRectangle(fillBrush, fillRectangle);
   }

   public void OnPaint(Graphics graphics, bool wasGood)
   {
      var color = getBackColor(wasGood);
      var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
      TextRenderer.DrawText(graphics, url, font, fillRectangle, color, flags);
   }
}