using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using Core.WinForms.ControlWrappers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls
{
   public class UiAction : UserControl
   {
      protected const string BUSY_TEXT_PROCESSOR_NOT_INITIALIZED = "BusyTextProcessor not initialized";
      protected const string PROGRESS_DEFINITE_PROCESSOR_NOT_INITIALIZED = "Progress Definite Processor not initialized";
      protected const string BUSY_PROCESSOR_NOT_INITIALIZED = "Busy Processor Not Initialized";

      protected static Hash<UiActionType, Color> globalForeColors;
      protected static Hash<UiActionType, Color> globalBackColors;
      protected static Hash<UiActionType, MessageStyle> globalStyles;

      static UiAction()
      {
         globalForeColors = new Hash<UiActionType, Color>
         {
            [UiActionType.Uninitialized] = Color.White,
            [UiActionType.Message] = Color.White,
            [UiActionType.Exception] = Color.White,
            [UiActionType.Success] = Color.White,
            [UiActionType.Failure] = Color.Black,
            [UiActionType.Selected] = Color.White,
            [UiActionType.Unselected] = Color.White,
            [UiActionType.ProgressIndefinite] = Color.White,
            [UiActionType.ProgressDefinite] = Color.White,
            [UiActionType.BusyText] = Color.White,
            [UiActionType.Automatic] = Color.Black,
            [UiActionType.Disabled] = Color.LightGray,
            [UiActionType.Caution] = Color.White,
            [UiActionType.ControlLabel] = Color.White,
            [UiActionType.Button] = Color.Black
         };
         globalBackColors = new Hash<UiActionType, Color>
         {
            [UiActionType.Uninitialized] = Color.Gray,
            [UiActionType.Message] = Color.Blue,
            [UiActionType.Exception] = Color.Red,
            [UiActionType.Success] = Color.Green,
            [UiActionType.Failure] = Color.Gold,
            [UiActionType.Selected] = Color.FromArgb(0, 127, 0),
            [UiActionType.Unselected] = Color.FromArgb(127, 0, 0),
            [UiActionType.Automatic] = Color.White,
            [UiActionType.Disabled] = Color.DarkGray,
            [UiActionType.Caution] = Color.CadetBlue,
            [UiActionType.ControlLabel] = Color.CadetBlue,
            [UiActionType.Button] = Color.LightGray
         };
         globalStyles = new Hash<UiActionType, MessageStyle>
         {
            [UiActionType.Uninitialized] = MessageStyle.Italic,
            [UiActionType.Message] = MessageStyle.None,
            [UiActionType.Exception] = MessageStyle.Bold,
            [UiActionType.Success] = MessageStyle.Bold,
            [UiActionType.Failure] = MessageStyle.Bold,
            [UiActionType.BusyText] = MessageStyle.ItalicBold,
            [UiActionType.Caution] = MessageStyle.Bold
         };
      }

      public static Hash<UiActionType, Color> GlobalForeColors => globalForeColors;

      public static Hash<UiActionType, Color> GlobalBackColors => globalBackColors;

      public static Hash<UiActionType, MessageStyle> GlobalStyles => globalStyles;

      protected Font italicFont;
      protected Font boldFont;
      protected Font italicBoldFont;
      protected AutoHash<UiActionType, Color> foreColors;
      protected AutoHash<UiActionType, Color> backColors;
      protected AutoHash<UiActionType, MessageStyle> styles;
      protected string text;
      protected UiActionType type;
      protected int value;
      protected Timer timerPaint;
      protected Timer timer;
      protected int maximum;
      protected int index;
      protected bool mouseInside;
      protected bool mouseDown;
      protected ToolTip toolTip;
      protected Maybe<string> _clickText;
      protected LateLazy<BusyTextProcessor> busyTextProcessor;
      protected LateLazy<ProgressDefiniteProcessor> progressDefiniteProcessor;
      protected LateLazy<BusyProcessor> busyProcessor;
      protected Maybe<int> _percentage;
      protected Maybe<Color> _foreColor;
      protected Maybe<Color> _backColor;
      protected Maybe<MessageStyle> _style;
      protected Maybe<UiActionType> _lastType;
      protected Maybe<bool> _lastEnabled;
      protected Maybe<Color> _lastForeColor;
      protected Maybe<Color> _lastBackColor;
      protected Maybe<MessageStyle> _lastStyle;
      protected Maybe<Image> _image;
      protected List<SubText> subTexts;
      protected Lazy<Stopwatch> stopwatch;
      protected Lazy<BackgroundWorker> backgroundWorker;
      protected Maybe<int> _labelWidth;
      protected Maybe<SubText> _legend;
      protected bool oneTimeTimer;

      public event EventHandler<AutomaticMessageArgs> AutomaticMessage;
      public event EventHandler<PaintEventArgs> Painting;
      public event EventHandler<PaintEventArgs> PaintingBackground;
      public event EventHandler Initialize;
      public event EventHandler<ArgumentsArgs> Arguments;
      public event DoWorkEventHandler DoWork;
      public event ProgressChangedEventHandler ProgressChanged;
      public event RunWorkerCompletedEventHandler RunWorkerCompleted;
      public event EventHandler Tick;

      public UiAction(Control control, bool center = false, bool is3D = true)
      {
         Center = center;
         Is3D = is3D;

         italicFont = new Font(base.Font, FontStyle.Italic);
         boldFont = new Font(base.Font, FontStyle.Bold);
         italicBoldFont = new Font(base.Font, FontStyle.Italic | FontStyle.Bold);

         foreColors = new AutoHash<UiActionType, Color>(mlt => globalForeColors[mlt]);
         backColors = new AutoHash<UiActionType, Color>(mlt => globalBackColors[mlt]);
         styles = new AutoHash<UiActionType, MessageStyle>(mlt => globalStyles[mlt]);
         text = string.Empty;
         _clickText = nil;
         type = UiActionType.Uninitialized;

         SetStyle(ControlStyles.UserPaint, true);
         SetStyle(ControlStyles.DoubleBuffer, true);
         SetStyle(ControlStyles.AllPaintingInWmPaint, true);

         _lastType = nil;
         _lastEnabled = nil;
         _lastForeColor = nil;
         _lastBackColor = nil;
         _lastStyle = nil;
         _image = nil;
         subTexts = new List<SubText>();
         CheckStyle = CheckStyle.None;
         stopwatch = new Lazy<Stopwatch>(() => new Stopwatch());

         timerPaint = new Timer
         {
            Interval = 100,
            Enabled = false
         };
         timerPaint.Tick += (_, _) =>
         {
            if (Enabled)
            {
               switch (type)
               {
                  case UiActionType.BusyText:
                     busyTextProcessor.Value.OnTick();
                     break;
                  case UiActionType.Automatic:
                  {
                     var args = new AutomaticMessageArgs();
                     AutomaticMessage?.Invoke(this, args);
                     if (args.GetText().Map(out var automaticText))
                     {
                        Text = automaticText;
                     }

                     break;
                  }
                  case UiActionType.Busy or UiActionType.ProgressIndefinite:
                     busyProcessor.Value.Advance();
                     break;
               }
            }

            this.Do(Refresh);
         };

         timer = new Timer
         {
            Interval = 1000,
            Enabled = false
         };
         timer.Tick += (_, _) =>
         {
            Tick?.Invoke(this, EventArgs.Empty);
            if (oneTimeTimer)
            {
               timer.Enabled = false;
               oneTimeTimer = false;
            }
         };

         Minimum = 1;
         maximum = 0;

         toolTip = new ToolTip { IsBalloon = true };
         toolTip.SetToolTip(this, "");

         busyTextProcessor = new LateLazy<BusyTextProcessor>(true, BUSY_TEXT_PROCESSOR_NOT_INITIALIZED);
         progressDefiniteProcessor = new LateLazy<ProgressDefiniteProcessor>(errorMessage: PROGRESS_DEFINITE_PROCESSOR_NOT_INITIALIZED);
         busyProcessor = new LateLazy<BusyProcessor>(true, BUSY_PROCESSOR_NOT_INITIALIZED);
         Resize += (_, _) =>
         {
            busyTextProcessor.ActivateWith(() => new BusyTextProcessor(Color.White, ClientRectangle));
            busyProcessor.ActivateWith(() => new BusyProcessor(ClientRectangle));
            progressDefiniteProcessor.Reset();
         };
         _percentage = nil;

         _foreColor = nil;
         _backColor = nil;
         _style = nil;

         backgroundWorker = new Lazy<BackgroundWorker>(() =>
         {
            var worker = new BackgroundWorker();
            worker.DoWork += (_, e) => DoWork?.Invoke(this, e);
            worker.ProgressChanged += (_, e) => ProgressChanged?.Invoke(this, e);
            worker.RunWorkerCompleted += (_, e) => RunWorkerCompleted?.Invoke(this, e);

            return worker;
         });

         Label = string.Empty;
         Line = false;
         _labelWidth = nil;

         control.Controls.Add(this);
         control.Resize += (_, _) => Refresh();

         _legend = nil;
      }

      public UiActionType Type => type;

      public bool Checked
      {
         get => CheckStyle == CheckStyle.Checked;
         set
         {
            CheckStyle = value ? CheckStyle.Checked : CheckStyle.Unchecked;
            Refresh();
         }
      }

      public CheckStyle CheckStyle { get; set; }

      public void SetForeColor(Color foreColor) => _foreColor = foreColor;

      public void SetBackColor(Color backColor) => _backColor = backColor;

      public void SetStyle(MessageStyle style) => _style = style;

      public override Font Font
      {
         get => base.Font;
         set
         {
            base.Font = value;
            italicFont = new Font(base.Font, FontStyle.Italic);
            boldFont = new Font(base.Font, FontStyle.Bold);
            italicBoldFont = new Font(base.Font, FontStyle.Italic | FontStyle.Bold);
         }
      }

      protected void setToolTip()
      {
         if (Clickable && ClickText.IsNotEmpty())
         {
            toolTip.SetToolTip(this, ClickText);
         }
         else
         {
            toolTip.SetToolTip(this, text);
         }
      }

      public override string Text
      {
         get => text;
         set
         {
            text = value;
            this.Do(setToolTip);
         }
      }

      public bool Center { get; set; }

      public bool Is3D { get; set; }

      public AutoHash<UiActionType, Color> ForeColors => foreColors;

      public AutoHash<UiActionType, Color> BackColors => backColors;

      public AutoHash<UiActionType, MessageStyle> Styles => styles;

      public Image Image
      {
         set
         {
            _image = value;
            refresh();
         }
      }

      public void ClearImage()
      {
         _image = nil;
         refresh();
      }

      public bool StretchImage { get; set; }

      public string Label { get; set; }

      public bool Line { get; set; }

      public Maybe<int> LabelWidth
      {
         get => _labelWidth;
         set => _labelWidth = value.Map(v => v.MaxOf(1).MinOf(Width));
      }

      protected Font getFont() => getStyle() switch
      {
         MessageStyle.None => Font,
         MessageStyle.Italic => italicFont,
         MessageStyle.Bold => boldFont,
         MessageStyle.ItalicBold => italicBoldFont,
         _ => Font
      };

      protected void refresh()
      {
         this.Do(() =>
         {
            Invalidate();
            Update();
         });
      }

      public void ShowMessage(string message, UiActionType type)
      {
         Busy(false);
         Text = message;
         this.type = type;

         refresh();
      }

      public void Uninitialized(string message) => ShowMessage(message, UiActionType.Uninitialized);

      public void Message(string message) => ShowMessage(message, UiActionType.Message);

      public void Exception(Exception exception) => ShowMessage(exception.Message, UiActionType.Exception);

      public void Success(string message) => ShowMessage(message, UiActionType.Success);

      public void Failure(string message) => ShowMessage(message, UiActionType.Failure);

      public void Caution(string message) => ShowMessage(message, UiActionType.Caution);

      public void Selected(string message) => ShowMessage(message, UiActionType.Selected);

      public void Unselected(string message) => ShowMessage(message, UiActionType.Unselected);

      public void Tape()
      {
         type = UiActionType.Tape;
         refresh();
      }

      public void ProgressText(string text)
      {
         Text = text;
         type = UiActionType.ProgressIndefinite;

         refresh();
      }

      public void Result(Result<(string, UiActionType)> _result)
      {
         if (_result.Map(out var message, out var messageProgressType, out var exception))
         {
            ShowMessage(message, messageProgressType);
         }
         else
         {
            Exception(exception);
         }
      }

      public void Result(Result<string> _result)
      {
         if (_result.Map(out var message, out var exception))
         {
            Success(message);
         }
         else
         {
            Exception(exception);
         }
      }

      public void LabeledText(string label, string text, Maybe<int> _lineWidth, bool line = false)
      {
         Label = label;
         Text = text;
         type = UiActionType.Labeled;
         LabelWidth = _lineWidth;
         Line = line;

         Refresh();
      }

      public void LabeledText(string label, string text, bool line = false) => LabeledText(label, text, nil, line);

      public void AttachTo(string text, Control control, string fontName = "Segoe UI", float fontSize = 9)
      {
         this.text = text;
         type = UiActionType.ControlLabel;

         control.Move += (_, _) =>
         {
            Location = new Point(control.Left, control.Top - Height + 4);
            Refresh();
         };

         using var font = new Font(fontName, fontSize);
         var size = TextRenderer.MeasureText(this.text, font);
         this.SetUp(control.Left, control.Top - size.Height + 1, size.Width + 20, size.Height, fontName, fontSize);

         Refresh();
      }

      public bool Clickable => _clickText;

      public string ClickText
      {
         get => _clickText.DefaultTo(() => text);
         set
         {
            _clickText = maybe(value.IsNotEmpty(), () => value);
            this.Do(setToolTip);
         }
      }

      public int Minimum { get; set; }

      public int Maximum
      {
         get => maximum;
         set
         {
            maximum = value;
            index = Minimum;
         }
      }

      public bool Stopwatch { get; set; }

      public void Progress(int value, string text = "", bool asPercentage = false)
      {
         if (asPercentage)
         {
            _percentage = value;
         }
         else
         {
            this.value = value;
         }

         Text = text;
         type = UiActionType.ProgressDefinite;

         refresh();
      }

      public void Progress(string text)
      {
         value = index++;

         Text = text;
         type = UiActionType.ProgressDefinite;

         refresh();
      }

      public void Busy(string text)
      {
         startStopwatch(true);

         Text = text;
         type = UiActionType.BusyText;

         this.Do(() => timerPaint.Enabled = true);
         refresh();
      }

      protected Color getForeColor(UiActionType type) => foreColors[type];

      protected Color getForeColor() => _foreColor.DefaultTo(() => foreColors[type]);

      protected Color getBackColor(UiActionType type) => backColors[type];

      protected Color getBackColor() => _backColor.DefaultTo(() => backColors[type]);

      protected MessageStyle getStyle(UiActionType type) => styles[type];

      protected MessageStyle getStyle() => _style.DefaultTo(() => styles[type]);

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         void paintStopwatch()
         {
            if (Stopwatch)
            {
               var elapsed = stopwatch.Value.Elapsed.ToString(@"mm\:ss");
               using var font = new Font("Consolas", 8);
               var size = TextRenderer.MeasureText(e.Graphics, elapsed, font);
               var location = new Point(ClientRectangle.Width - size.Width - 20, 4);
               var rectangle = new Rectangle(location, size);
               TextRenderer.DrawText(e.Graphics, elapsed, font, rectangle, Color.White);
               using var pen = new Pen(Color.White);
               e.Graphics.DrawRectangle(pen, rectangle);
            }
         }

         var checkStyle = type switch
         {
            UiActionType.Busy or UiActionType.BusyText => CheckStyle.None,
            _ => CheckStyle
         };
         var writer = new UiActionWriter(Center, checkStyle)
         {
            Rectangle = ClientRectangle,
            Font = getFont(),
            Color = getForeColor()
         };

         switch (type)
         {
            case UiActionType.ProgressIndefinite:
               writer.Write(text, e.Graphics);
               break;
            case UiActionType.Busy:
               busyProcessor.Value.OnPaint(e.Graphics);
               paintStopwatch();
               break;
            case UiActionType.ProgressDefinite:
            {
               var percentText = $"{getPercentage()}%";
               writer.Rectangle = progressDefiniteProcessor.Value.PercentRectangle;
               writer.Center(true);
               writer.Color = Color.Black;
               writer.Write(percentText, e.Graphics);

               writer.Rectangle = progressDefiniteProcessor.Value.TextRectangle;
               writer.Color = getForeColor();
               writer.Write(text, e.Graphics);
               break;
            }
            case UiActionType.BusyText:
            {
               writer.Rectangle = busyTextProcessor.Value.TextRectangle;
               writer.Center(true);
               writer.Write(text, e.Graphics);
               paintStopwatch();
               break;
            }
            case UiActionType.Labeled:
            {
               var processor = new LabelProcessor(Label, text, Line, _labelWidth, getFont());
               processor.OnPaint(e.Graphics, ClientRectangle);
               break;
            }
            case UiActionType.ControlLabel:
               writer.Write(text, e.Graphics);
               break;
            default:
            {
               if (type != UiActionType.Tape)
               {
                  writer.Write(text, e.Graphics);
               }

               break;
            }
         }

         if (type is not UiActionType.Busy && type is not UiActionType.BusyText)
         {
            var foreColor = getForeColor();
            var backColor = getBackColor();
            foreach (var subText in subTexts)
            {
               subText.Draw(e.Graphics, foreColor, backColor);
            }

            if (_legend.Map(out var legend))
            {
               legend.Draw(e.Graphics, foreColor, backColor);
            }
         }

         if (Clickable)
         {
            var color = getForeColor();
            using var pen = new Pen(color, 4);
            e.Graphics.DrawLine(pen, ClientRectangle.Right - 4, 4, ClientRectangle.Right - 4, ClientRectangle.Bottom - 4);

            if (mouseInside || mouseDown)
            {
               using var dashedPen = new Pen(color, 1);
               dashedPen.DashStyle = DashStyle.Dash;
               var rectangle = ClientRectangle;
               rectangle.Inflate(-2, -2);
               drawRectangle(e.Graphics, dashedPen, rectangle);
            }
         }

         Painting?.Invoke(this, e);
      }

      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
         base.OnPaintBackground(pevent);

         switch (type)
         {
            case UiActionType.Tape:
            {
               using var brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);
               break;
            }
            case UiActionType.ProgressIndefinite or UiActionType.Busy:
            {
               using var brush = new SolidBrush(Color.DarkSlateGray);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);
               break;
            }
            case UiActionType.ProgressDefinite:
            {
               progressDefiniteProcessor.ActivateWith(() => new ProgressDefiniteProcessor(Font, pevent.Graphics, ClientRectangle));
               progressDefiniteProcessor.Value.OnPaint(pevent.Graphics);
               var textRectangle = progressDefiniteProcessor.Value.TextRectangle;

               using var coralBrush = new SolidBrush(Color.Coral);
               fillRectangle(pevent.Graphics, coralBrush, textRectangle);
               var width = textRectangle.Width;
               var percentWidth = getPercentage(width);
               var location = textRectangle.Location;
               var size = new Size(percentWidth, textRectangle.Height);
               var rectangle = new Rectangle(location, size);
               using var cornflowerBlueBrush = new SolidBrush(Color.CornflowerBlue);
               fillRectangle(pevent.Graphics, cornflowerBlueBrush, rectangle);
               break;
            }
            case UiActionType.Unselected:
            {
               using var brush = new SolidBrush(Color.White);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);

               using var pen = new Pen(Color.DarkGray, 10);
               drawRectangle(pevent.Graphics, pen, ClientRectangle);
               break;
            }
            case UiActionType.Selected:
            {
               using var brush = new SolidBrush(Color.White);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);

               using var pen = new Pen(Color.Black, 10);
               drawRectangle(pevent.Graphics, pen, ClientRectangle);
               break;
            }
            case UiActionType.BusyText:
            {
               using var brush = new SolidBrush(Color.Teal);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);

               busyTextProcessor.Value.OnPaint(pevent);

               break;
            }
            case UiActionType.Labeled:
            {
               var processor = new LabelProcessor(Label, text, Line, _labelWidth, getFont());
               processor.OnPaintBackground(pevent.Graphics, ClientRectangle);
               break;
            }
            case UiActionType.ControlLabel:
            {
               using var brush = new SolidBrush(Color.CadetBlue);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);
               break;
            }
            default:
            {
               var backColor = getBackColor();
               using var brush = new SolidBrush(backColor);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);
               break;
            }
         }

         if (Is3D)
         {
            using var darkGrayPen = new Pen(Color.DarkGray, 1);
            using var lightPen = new Pen(Color.White, 1);

            var left = ClientRectangle.Left;
            var top = ClientRectangle.Top;
            var width = ClientRectangle.Width - 1;
            var height = ClientRectangle.Height - 1;

            pevent.Graphics.DrawLine(darkGrayPen, new Point(left, top), new Point(width, top));
            pevent.Graphics.DrawLine(darkGrayPen, new Point(left, top), new Point(left, height));
            pevent.Graphics.DrawLine(lightPen, new Point(left, height), new Point(width, height));
            pevent.Graphics.DrawLine(lightPen, new Point(width, top), new Point(width, height));
         }

         if (_image.Map(out var image))
         {
            if (StretchImage)
            {
               pevent.Graphics.DrawImage(image, ClientRectangle with { X = 0, Y = 0 });
            }
            else
            {
               pevent.Graphics.DrawImage(image, Point.Empty);
            }
         }

         PaintingBackground?.Invoke(this, pevent);
      }

      protected override void OnMouseEnter(EventArgs e)
      {
         base.OnMouseEnter(e);

         if (!mouseInside)
         {
            mouseInside = true;
            refresh();
         }
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);

         if (mouseInside)
         {
            mouseInside = false;
            refresh();
         }
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         base.OnMouseDown(e);

         if (!mouseDown)
         {
            mouseDown = true;
            refresh();
         }
      }

      protected override void OnMouseUp(MouseEventArgs e)
      {
         base.OnMouseUp(e);

         if (mouseDown)
         {
            mouseDown = false;
            refresh();
         }
      }

      protected int getPercentage() => _percentage.DefaultTo(() => (int)((float)value / maximum * 100));

      protected int getPercentage(int width) => (int)((float)value / maximum * width);

      public int Index(bool increment) => increment ? index++ : index;

      protected void startStopwatch(bool enabled)
      {
         if (Stopwatch)
         {
            if (enabled)
            {
               stopwatch.Value.Restart();
            }
            else
            {
               stopwatch.Value.Stop();
            }
         }
      }

      public void Busy(bool enabled)
      {
         Text = "";
         type = UiActionType.Busy;

         this.Do(() => timerPaint.Enabled = enabled);
      }

      public void Button(string text)
      {
         Text = text;
         type = UiActionType.Button;

         Refresh();
      }

      public void StartAutomatic()
      {
         Text = "";
         type = UiActionType.Automatic;

         this.Do(() => timerPaint.Enabled = true);
      }

      public void StopAutomatic()
      {
         this.Do(() => timerPaint.Enabled = false);
      }

      public bool IsAutomaticRunning => timerPaint.Enabled;

      protected override void OnEnabledChanged(EventArgs e)
      {
         base.OnEnabledChanged(e);

         if (Enabled)
         {
            if (_lastType.Map(out var lastType))
            {
               ShowMessage(text, lastType);
               _lastType = nil;
            }
            else
            {
               ShowMessage(text, UiActionType.Uninitialized);
            }

            if (_lastEnabled.Map(out var enabled))
            {
               timerPaint.Enabled = enabled;
               _lastEnabled = nil;
            }

            _foreColor = _lastForeColor;
            _backColor = _lastBackColor;
            _style = _lastStyle;
         }
         else
         {
            _lastType = type;
            type = UiActionType.Disabled;

            _lastEnabled = timerPaint.Enabled;
            timerPaint.Enabled = false;

            _lastForeColor = _foreColor;
            _foreColor = nil;

            _lastBackColor = _backColor;
            _backColor = nil;

            _lastStyle = _style;
            _style = nil;

            refresh();
         }
      }

      protected virtual void drawRectangle(Graphics graphics, Pen pen, Rectangle rectangle) => graphics.DrawRectangle(pen, rectangle);

      protected virtual void fillRectangle(Graphics graphics, Brush brush, Rectangle rectangle) => graphics.FillRectangle(brush, rectangle);

      public SubText SubText(string text, int x, int y)
      {
         var subText = new SubText(text, x, y, getForeColor(), getBackColor());
         subTexts.Add(subText);

         return subText;
      }

      public void RemoveSubTextAt(int index) => subTexts.RemoveAt(index);

      public void ClearSubTexts() => subTexts.Clear();

      public void RunAsync()
      {
         var args = new ArgumentsArgs();
         Arguments?.Invoke(this, args);
         if (args.Arguments.Map(out var arguments))
         {
            RunWorkerAsync(arguments);
         }
         else
         {
            RunWorkerAsync();
         }
      }

      public void RunWorkerAsync()
      {
         Initialize?.Invoke(this, EventArgs.Empty);
         if (!backgroundWorker.Value.IsBusy)
         {
            backgroundWorker.Value.RunWorkerAsync();
         }
      }

      public void RunWorkerAsync(object argument)
      {
         Initialize?.Invoke(this, EventArgs.Empty);
         if (!backgroundWorker.Value.IsBusy)
         {
            backgroundWorker.Value.RunWorkerAsync(argument);
         }
      }

      public bool IsBusy => backgroundWorker.Value.IsBusy;

      public bool WorkReportsProgress
      {
         get => backgroundWorker.Value.WorkerReportsProgress;
         set => backgroundWorker.Value.WorkerReportsProgress = value;
      }

      public void ReportProgress(int percentProgress, object userState) => backgroundWorker.Value.ReportProgress(percentProgress, userState);

      public void ReportProgress(int percentProgress) => backgroundWorker.Value.ReportProgress(percentProgress);

      public void CancelAsync() => backgroundWorker.Value.CancelAsync();

      public bool CancellationPending => backgroundWorker.Value.CancellationPending;

      public void StartTimer() => timer.Enabled = true;

      public void StartTimer(TimeSpan interval, bool oneTimeTimer = false)
      {
         this.oneTimeTimer = oneTimeTimer;
         timer.Interval = (int)interval.TotalMilliseconds;
         timer.Enabled = true;
      }

      public void StopTimer() => timer.Enabled = false;

      public void PerformClick() => OnClick(EventArgs.Empty);

      public bool TimerEnabled => timer.Enabled;

      protected (int, int) legendLocation() => CheckStyle switch
      {
         CheckStyle.None => (2, 2),
         _ => (20, 2)
      };

      public SubText Legend(string text, bool useControlForeColor = true)
      {
         var (x, y) = legendLocation();
         var legend = SubText(text, x, y).Set.FontSize(8).Outline(true).UseControlForeColor(useControlForeColor).End;
         _legend = legend;

         return legend;
      }

      public SubText SuccessLegend(string text)
      {
         return Legend(text, false).Set.ForeColor(Color.White).BackColor(Color.Green).End;
      }

      public SubText FailureLegend(string text)
      {
         return Legend(text, false).Set.ForeColor(Color.Black).BackColor(Color.Gold).End;
      }

      public void Legend()
      {
         _legend = nil;
         refresh();
      }
   }
}