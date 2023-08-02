using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Collections;
using Core.Computers;
using Core.DataStructures;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Numbers;
using Core.Strings;
using Core.WinForms.ControlWrappers;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiAction : UserControl
{
   public class CheckToggler
   {
      protected Hash<Guid, string> uiActions;
      protected AutoStringHash<List<UiAction>> groups;
      protected Result<string> _group;

      public CheckToggler()
      {
         uiActions = new Hash<Guid, string>();
         groups = new AutoStringHash<List<UiAction>>(true, _ => new List<UiAction>(), true);
         _group = fail("Group not set");
      }

      public CheckToggler Group(string group)
      {
         _group = group;
         return this;
      }

      public CheckToggler Group(Control control) => Group(control.Name);

      protected Maybe<List<UiAction>> getUiActionList(Guid id)
      {
         return
            from @group in uiActions.Items[id]
            from list in groups.Items[@group]
            select list;
      }

      protected void checkChanged(CheckStyleChangedArgs e)
      {
         var id = e.Id;
         if (e.CheckStyle == CheckStyle.Checked)
         {
            var _actionList = getUiActionList(id);
            if (_actionList is (true, var actionList))
            {
               foreach (var uiAction in actionList.Where(uiAction => uiAction.Id != id))
               {
                  uiAction.SetCheckStyle(CheckStyle.None);
               }
            }
         }
      }

      public CheckToggler Add(UiAction uiAction)
      {
         uiAction.CheckStyleChanged += (_, e) => checkChanged(e);
         uiActions[uiAction.Id] = _group;
         groups[_group].Add(uiAction);

         return this;
      }

      public CheckToggler Add(params UiAction[] uiActions) => AddRange(uiActions);

      public CheckToggler AddRange(IEnumerable<UiAction> uiActions)
      {
         foreach (var uiAction in uiActions)
         {
            Add(uiAction);
         }

         return this;
      }
   }

   public class LabelSetter
   {
      protected UiAction uiAction;

      public LabelSetter(UiAction uiAction)
      {
         this.uiAction = uiAction;
      }

      public LabelSetter Label(string label)
      {
         uiAction._label = label.NotEmpty();
         return this;
      }

      public LabelSetter Label()
      {
         uiAction._label = nil;
         return this;
      }

      public LabelSetter LabelWidth(int labelWidth)
      {
         uiAction._labelWidth = labelWidth;
         return this;
      }

      public LabelSetter LabelWidth()
      {
         uiAction._labelWidth = nil;
         return this;
      }

      public UiAction End => uiAction;
   }

   protected const float START_AMOUNT = .9f;
   protected const string CLICK_TO_CANCEL = "🖰 ⇒ 🛑";

   protected static Hash<UiActionType, Color> globalForeColors;
   protected static Hash<UiActionType, Color> globalBackColors;
   protected static Hash<UiActionType, MessageStyle> globalStyles;
   protected static Lazy<CheckToggler> toggler;
   protected static BusyStyle busyStyle;

   static UiAction()
   {
      globalForeColors = new Hash<UiActionType, Color>
      {
         [UiActionType.Uninitialized] = Color.White,
         [UiActionType.Message] = Color.White,
         [UiActionType.Exception] = Color.White,
         [UiActionType.Success] = Color.White,
         [UiActionType.Failure] = Color.Black,
         [UiActionType.NoStatus] = Color.Black,
         [UiActionType.Selected] = Color.White,
         [UiActionType.Unselected] = Color.White,
         [UiActionType.ProgressIndefinite] = Color.White,
         [UiActionType.ProgressDefinite] = Color.White,
         [UiActionType.BusyText] = Color.White,
         [UiActionType.Automatic] = Color.Black,
         [UiActionType.Disabled] = Color.LightGray,
         [UiActionType.Caution] = Color.White,
         [UiActionType.ControlLabel] = Color.White,
         [UiActionType.Button] = Color.Black,
         [UiActionType.Console] = Color.White,
         [UiActionType.Busy] = Color.White,
         [UiActionType.MuteProgress] = Color.White
      };
      globalBackColors = new Hash<UiActionType, Color>
      {
         [UiActionType.Uninitialized] = Color.Gray,
         [UiActionType.Message] = Color.Blue,
         [UiActionType.Exception] = Color.Red,
         [UiActionType.Success] = Color.Green,
         [UiActionType.Failure] = Color.Gold,
         [UiActionType.NoStatus] = Color.White,
         [UiActionType.Selected] = Color.FromArgb(0, 127, 0),
         [UiActionType.Unselected] = Color.FromArgb(127, 0, 0),
         [UiActionType.Automatic] = Color.White,
         [UiActionType.Disabled] = Color.DarkGray,
         [UiActionType.Caution] = Color.CadetBlue,
         [UiActionType.ControlLabel] = Color.CadetBlue,
         [UiActionType.Button] = Color.LightGray,
         [UiActionType.Console] = Color.Blue
      };
      globalStyles = new Hash<UiActionType, MessageStyle>
      {
         [UiActionType.Uninitialized] = MessageStyle.Italic,
         [UiActionType.Message] = MessageStyle.None,
         [UiActionType.Exception] = MessageStyle.Bold,
         [UiActionType.Success] = MessageStyle.Bold,
         [UiActionType.Failure] = MessageStyle.Bold,
         [UiActionType.NoStatus] = MessageStyle.Bold,
         [UiActionType.BusyText] = MessageStyle.ItalicBold,
         [UiActionType.Caution] = MessageStyle.Bold,
         [UiActionType.ControlLabel] = MessageStyle.Bold,
         [UiActionType.Http] = MessageStyle.Bold
      };
      toggler = new Lazy<CheckToggler>(() => new CheckToggler());
      MainForm = nil;
      busyStyle = BusyStyle.Default;
   }

   public static Hash<UiActionType, Color> GlobalForeColors => globalForeColors;

   public static Hash<UiActionType, Color> GlobalBackColors => globalBackColors;

   public static Hash<UiActionType, MessageStyle> GlobalStyles => globalStyles;

   public static CheckToggler Toggler => toggler.Value;

   public static Maybe<Form> MainForm { get; set; }

   public static BusyStyle BusyStyle
   {
      get => busyStyle;
      set => busyStyle = value;
   }

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
   protected UiToolTip toolTip;
   protected Maybe<string> _clickText;
   protected LazyMaybe<BusyTextProcessor> _busyTextProcessor;
   protected LazyMaybe<ProgressDefiniteProcessor> _progressDefiniteProcessor;
   protected LazyMaybe<BusyProcessor> _busyProcessor;
   protected LazyMaybe<LabelProcessor> _labelProcessor;
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
   protected Hash<Guid, SubText> subTexts;
   protected Lazy<Stopwatch> stopwatch;
   protected Lazy<BackgroundWorker> backgroundWorker;
   protected Maybe<string> _label;
   protected Maybe<int> _labelWidth;
   protected bool oneTimeTimer;
   protected Maybe<SubText> _working;
   protected Timer workingTimer;
   protected MaybeStack<SubText> legends;
   protected bool isDirty;
   protected CheckStyle checkStyle;
   protected Guid id;
   protected bool httpHandlerAdded;
   protected bool isUrlGood;
   protected Lazy<Font> marqueeFont;
   protected Lazy<UiActionScroller> scroller;
   protected Maybe<string> _failureToolTip;
   protected Maybe<string> _exceptionToolTip;
   protected Maybe<string> _noStatusToolTip;
   protected Maybe<SubText> _failureSubText;
   protected Maybe<SubText> _exceptionSubText;
   protected Maybe<SubText> _noStatusSubText;
   protected Maybe<string> _oldTitle;
   protected Maybe<SubText> _progressSubText;
   protected bool flipOn;
   protected Maybe<SubText> _flipFlop;
   protected bool clickToCancel;
   protected Maybe<TaskBarProgress> _taskBarProgress;
   protected bool cancelled;
   protected Rectangle[] rectangles;
   protected Maybe<int> _floor;
   protected Maybe<int> _ceiling;

   public event EventHandler<AutomaticMessageArgs> AutomaticMessage;
   public event EventHandler<PaintEventArgs> Painting;
   public event EventHandler<PaintEventArgs> PaintingBackground;
   public event EventHandler<InitializeArgs> Initialize;
   public event EventHandler<ArgumentsArgs> Arguments;
   public event DoWorkEventHandler DoWork;
   public event ProgressChangedEventHandler ProgressChanged;
   public event RunWorkerCompletedEventHandler RunWorkerCompleted;
   public event EventHandler Tick;
   public event EventHandler<ValidatedArgs> ValidateText;
   public event EventHandler<CheckStyleChangedArgs> CheckStyleChanged;
   public event EventHandler<AppearanceOverrideArgs> AppearanceOverride;
   public new event EventHandler TextChanged;
   public event EventHandler<MessageShownArgs> MessageShown;
   public event EventHandler<DrawToolTipEventArgs> PaintToolTip;
   public event EventHandler<UiActionRectangleArgs> ClickOnRectangle;
   public event EventHandler<UiActionRectangleArgs> MouseMoveOnRectangle;
   public event EventHandler<UiActionRectanglePaintArgs> PaintOnRectangle;

   public UiAction(Control control, bool is3D = false)
   {
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
      subTexts = new Hash<Guid, SubText>();
      checkStyle = CheckStyle.None;
      MessageAlignment = CardinalAlignment.Center;
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
               case UiActionType.BusyText when _busyTextProcessor is (true, var busyTextProcessor):
                  busyTextProcessor.OnTick();
                  break;
               case UiActionType.Automatic:
               {
                  var args = new AutomaticMessageArgs();
                  AutomaticMessage?.Invoke(this, args);
                  var _automaticText = args.GetText();
                  if (_automaticText)
                  {
                     Text = _automaticText;
                  }

                  break;
               }
               case UiActionType.Busy or UiActionType.ProgressIndefinite when _busyProcessor is (true, var busyProcessor):
                  busyProcessor.Advance();
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

      toolTip = new UiToolTip(this);
      toolTip.SetToolTip(this, "");

      _busyTextProcessor = lazy.maybe<BusyTextProcessor>();
      _progressDefiniteProcessor = lazy.maybe<ProgressDefiniteProcessor>();
      _busyProcessor = lazy.maybe<BusyProcessor>();
      _labelProcessor = lazy.maybe<LabelProcessor>();

      Resize += (_, _) =>
      {
         _busyTextProcessor.Reset();
         _progressDefiniteProcessor.Reset();
         _busyProcessor.Reset();
         _labelProcessor.Reset();
      };
      _percentage = nil;

      Click += (_, _) =>
      {
         if (clickToCancel && !cancelled)
         {
            cancelled = true;
         }
      };

      Click += (_, _) =>
      {
         var location = PointToClient(Cursor.Position);
         for (var i = 0; i < rectangles.Length; i++)
         {
            if (rectangles[i].Contains(location))
            {
               ClickOnRectangle?.Invoke(this, new UiActionRectangleArgs(i, location));
               return;
            }
         }
      };

      MouseMove += (_, _) =>
      {
         var location = PointToClient(Cursor.Position);
         for (var i = 0; i < rectangles.Length; i++)
         {
            if (rectangles[i].Contains(location))
            {
               MouseMoveOnRectangle?.Invoke(this, new UiActionRectangleArgs(i, location));
               return;
            }
         }
      };

      Resize += (_, _) => determineFloorAndCeiling();

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

      _label = nil;
      _labelWidth = nil;

      control.Controls.Add(this);
      control.Resize += (_, _) => Refresh();

      legends = new MaybeStack<SubText>();
      _working = nil;
      EmptyTextTitle = nil;

      workingTimer = new Timer { Interval = 1000 };
      workingTimer.Tick += (_, _) =>
      {
         if (_working)
         {
            _working = nil;
         }
         else
         {
            _working = getWorking();
         }

         Refresh();
      };

      checkStyle = CheckStyle.None;
      id = Guid.NewGuid();
      httpHandlerAdded = false;
      isUrlGood = false;
      marqueeFont = new Lazy<Font>(() => new Font("Consolas", 8));
      scroller = new Lazy<UiActionScroller>(() => new UiActionScroller(marqueeFont.Value, getClientRectangle(nil), getForeColor(), getBackColor()));
      _failureToolTip = nil;
      _exceptionToolTip = nil;
      _noStatusToolTip = nil;
      _failureSubText = nil;
      _exceptionSubText = nil;
      _noStatusSubText = nil;
      _oldTitle = nil;
      _progressSubText = nil;
      _flipFlop = nil;
      clickToCancel = false;

      ClickGlyph = true;
      CardinalAlignment = CardinalAlignment.Center;
      _taskBarProgress = nil;
      cancelled = true;
      rectangles = Array.Empty<Rectangle>();
      _floor = nil;
      _ceiling = nil;
   }

   public bool AutoSizeText { get; set; }

   protected BusyProcessor getBusyProcessor(Rectangle clientRectangle) => busyStyle switch
   {
      BusyStyle.Default => new DefaultBusyProcessor(clientRectangle),
      BusyStyle.Sine => new SineBusyProcessor(clientRectangle),
      BusyStyle.Rectangle => new RectangleBusyProcessor(clientRectangle),
      BusyStyle.BarberPole => new BarberPoleBusyProcessor(clientRectangle),
      _ => new DefaultBusyProcessor(clientRectangle)
   };

   protected void activateProcessor(Graphics graphics)
   {
      if (_label is (true, var label))
      {
         _labelProcessor.Activate(new LabelProcessor(label, _labelWidth, getFont(), EmptyTextTitle, graphics, ClientRectangle));
      }
      else
      {
         _labelProcessor.Reset();
      }

      switch (type)
      {
         case UiActionType.BusyText:
         {
            var clientRectangle = getRectangle();
            _busyTextProcessor.Activate(() => new BusyTextProcessor(Color.White, clientRectangle));
            break;
         }
         case UiActionType.Busy:
         {
            var clientRectangle = getRectangle();
            _busyProcessor.Activate(() => getBusyProcessor(clientRectangle));
            break;
         }
         case UiActionType.ProgressDefinite:
         {
            var clientRectangle = getRectangle();
            _progressDefiniteProcessor.Activate(() => new ProgressDefiniteProcessor(Font, graphics, clientRectangle));
            break;
         }
      }
   }

   public Guid Id => id;

   public bool Cancelled
   {
      get => cancelled;
      set => cancelled = value;
   }

   protected SubText getWorking()
   {
      using var font = new Font("Consolas", 8);
      var size = TextRenderer.MeasureText("working", font);
      var y = ClientSize.Height - size.Height - 4;

      return new SubText("working", 4, y, ClientSize, ClickGlyph).Set.FontSize(8).Invert().Outline().End;
   }

   public UiActionType Type
   {
      get => type;
      set
      {
         type = value;
         refresh();
      }
   }

   public bool ClickToCancel
   {
      get => clickToCancel;
      set
      {
         clickToCancel = value;
         refresh();
      }
   }

   public bool TaskBarProgress { get; set; }

   public bool Checked
   {
      get => CheckStyle == CheckStyle.Checked;
      set => CheckStyle = value ? CheckStyle.Checked : CheckStyle.Unchecked;
   }

   public CheckStyle CheckStyle
   {
      get => checkStyle;
      set
      {
         checkStyle = value;
         CheckStyleChanged?.Invoke(this, new CheckStyleChangedArgs(id, checkStyle));
         determineFloorAndCeiling();
         Refresh();
      }
   }

   public CardinalAlignment MessageAlignment { get; set; }

   public bool IsFile { get; set; }

   internal void SetCheckStyle(CheckStyle checkStyle)
   {
      this.checkStyle = checkStyle;
      Refresh();
   }

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
         toolTip.Font = value;
      }
   }

   protected void setToolTip()
   {
      if (PaintToolTip is not null)
      {
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) => PaintToolTip.Invoke(this, e));
      }
      else if (_failureToolTip is (true, var failureToolTip))
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "failure";
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, failureToolTip, toolTip.Font, Color.Black, Color.Gold, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Gold, Color.Black, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, failureToolTip));
      }
      else if (_exceptionToolTip is (true, var exceptionToolTip))
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "exception";
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, exceptionToolTip, toolTip.Font, Color.White, Color.Red, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Red, Color.White, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, exceptionToolTip));
      }
      else if (_noStatusToolTip is (true, var noStatusToolTip))
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "no status";
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, noStatusToolTip, toolTip.Font, Color.Black, Color.White, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.White, Color.Black, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, noStatusToolTip));
      }
      else if (type == UiActionType.Failure)
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "failure";
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, text, toolTip.Font, Color.Black, Color.Gold, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Gold, Color.Black, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, text));
      }
      else if (type == UiActionType.Exception)
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "exception";
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, text, toolTip.Font, Color.White, Color.Red, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Red, Color.White, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, text));
      }
      else if (type == UiActionType.NoStatus)
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "no status";
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, text, toolTip.Font, Color.Black, Color.White, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.White, Color.Black, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, text));
      }
      else if (Clickable && ClickText.IsNotEmpty())
      {
         if (_oldTitle is (true, var oldTitle))
         {
            toolTip.ToolTipTitle = oldTitle;
            _oldTitle = nil;
         }
         else
         {
            toolTip.ToolTipTitle = "";
         }

         toolTip.Action = nil;
         this.Do(() => toolTip.SetToolTip(this, ClickText));
      }
      else
      {
         if (_oldTitle is (true, var oldTitle))
         {
            toolTip.ToolTipTitle = oldTitle;
            _oldTitle = nil;
         }
         else
         {
            toolTip.ToolTipTitle = "";
         }

         toolTip.Action = nil;
         this.Do(() => toolTip.SetToolTip(this, text));
      }

      refresh();
   }

   public override string Text
   {
      get => text;
      set
      {
         text = value;
         this.Do(setToolTip);
         TextChanged?.Invoke(this, EventArgs.Empty);
      }
   }

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

   public CardinalAlignment CardinalAlignment { get; set; }

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
         determineFloorAndCeiling();
         Invalidate();
         Update();
      });
   }

   public void ShowMessage(string message, UiActionType type)
   {
      FloatingException(false);
      Busy(false);
      Working = false;
      this.type = type;
      Text = message;

      MessageShown?.Invoke(this, new MessageShownArgs(text, this.type));

      if (type == UiActionType.Http)
      {
         if (!httpHandlerAdded)
         {
            Click += openUrl;
            httpHandlerAdded = true;
         }
      }
      else if (httpHandlerAdded)
      {
         Click -= openUrl;
         httpHandlerAdded = false;
      }

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.State = WinForms.Controls.TaskBarProgress.TaskBarState.NoProgress;
      }

      _taskBarProgress = nil;

      refresh();
   }

   public void Display(string message, Color foreColor, Color backColor)
   {
      ForeColor = foreColor;
      BackColor = backColor;
      ShowMessage(message, UiActionType.Display);
   }

   public void Uninitialized(string message) => ShowMessage(message, UiActionType.Uninitialized);

   public void Message(string message) => ShowMessage(message, UiActionType.Message);

   public void Exception(Exception exception)
   {
      ShowMessage(exception.Message, UiActionType.Exception);
   }

   public void Success(string message) => ShowMessage(message, UiActionType.Success);

   public void Failure(string message) => ShowMessage(message, UiActionType.Failure);

   public void NoStatus(string message) => ShowMessage(message, UiActionType.NoStatus);

   public void Caution(string message) => ShowMessage(message, UiActionType.Caution);

   public void Selected(string message) => ShowMessage(message, UiActionType.Selected);

   public void Unselected(string message) => ShowMessage(message, UiActionType.Unselected);

   public void FileName(FileName file, bool checkForFileExistence = true)
   {
      try
      {
         IsFile = true;
         if (checkForFileExistence)
         {
            if (file)
            {
               Success(file.FullPath);
            }
            else
            {
               Failure(file.FullPath);
            }
         }
         else
         {
            Message(file.FullPath);
         }
      }
      catch (Exception exception)
      {
         Exception(exception);
      }
   }

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
      if (_result is (true, var (message, messageProgressType)))
      {
         ShowMessage(message, messageProgressType);
      }
      else
      {
         Exception(_result.Exception);
      }
   }

   public void Result(Result<string> _result)
   {
      if (_result)
      {
         Success(_result);
      }
      else
      {
         Exception(_result.Exception);
      }
   }

   public void Result(Result<(string label, string text)> _result, int labelWidth)
   {
      if (_result is (true, var (label, message)))
      {
         Label(label).LabelWidth(labelWidth).End.Success(message);
      }
      else
      {
         _label = nil;
         Exception(_result.Exception);
      }
   }

   public void Result(Result<(string label, string text)> _result)
   {
      if (_result is (true, var (label, message)))
      {
         Label(label).End.Success(message);
      }
      else
      {
         _label = nil;
         Exception(_result.Exception);
      }
   }

   public void Optional(Optional<string> _message, string nilMessage)
   {
      if (_message is (true, var message))
      {
         Success(message);
      }
      else if (_message.AnyException)
      {
         Exception(_message.Exception);
      }
      else
      {
         Failure(nilMessage);
      }
   }

   public void Optional(Optional<string> _message, Func<string> nilMessageFunc)
   {
      if (_message is (true, var message))
      {
         Success(message);
      }
      else if (_message.AnyException)
      {
         Exception(_message.Exception);
      }
      else
      {
         Failure(nilMessageFunc());
      }
   }

   public void Optional(Optional<string> _message)
   {
      if (_message is (true, var message))
      {
         Success(message);
      }
      else if (_message.AnyException)
      {
         Exception(_message.Exception);
      }
   }

   public void AttachTo(string text, Control control, string fontName = "Segoe UI", float fontSize = 9, int left = -1, bool stretch = false,
      int width = -1)
   {
      this.text = text;
      type = UiActionType.ControlLabel;

      MessageShown?.Invoke(this, new MessageShownArgs(this.text, type));

      control.Move += (_, _) =>
      {
         Location = new Point(control.Left, control.Top - Height + 1);
         Refresh();
      };

      using var font = new Font(fontName, fontSize);
      var size = TextRenderer.MeasureText(this.text, font);

      if (left == -1)
      {
         left = control.Left;
      }

      if (width == -1)
      {
         width = stretch ? control.Width - left : size.Width + 20;
      }

      this.SetUp(left, control.Top - size.Height - 3, width, size.Height + 4, fontName, fontSize);

      Refresh();
   }

   public bool Clickable => _clickText;

   public string ClickText
   {
      get => _clickText | text;
      set
      {
         _clickText = maybe<string>() & value.IsNotEmpty() & value;
         this.Do(setToolTip);
      }
   }

   public int Minimum { get; set; }

   protected IntPtr getHandle()
   {
      if (MainForm is (true, var mainForm))
      {
         return mainForm.Get(() => mainForm.Handle);
      }

      return ParentForm.Get(() => ParentForm.Handle);
   }

   protected TaskBarProgress getTaskBarProgress(int value) => new(getHandle(), value);

   protected TaskBarProgress getTaskBarProgress()
   {
      var taskBarProgress = getTaskBarProgress(0);
      taskBarProgress.State = WinForms.Controls.TaskBarProgress.TaskBarState.Indeterminate;
      return taskBarProgress;
   }

   public int Maximum
   {
      get => maximum;
      set
      {
         maximum = value;
         index = Minimum;
         _taskBarProgress = maybe<TaskBarProgress>() & TaskBarProgress & (() => getTaskBarProgress(value));
      }
   }

   public bool Stopwatch { get; set; }

   public bool StopwatchInverted { get; set; }

   public Maybe<TimeSpan> Elapsed => maybe<TimeSpan>() & Stopwatch & (() => stopwatch.Value.Elapsed);

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
      Working = false;

      MessageShown?.Invoke(this, new MessageShownArgs(Text, type));

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.Value = value;
      }

      refresh();
   }

   public void Progress(string text)
   {
      value = index++;

      Text = text;
      type = UiActionType.ProgressDefinite;
      Working = false;

      MessageShown?.Invoke(this, new MessageShownArgs(Text, type));

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.Value = value;
      }

      refresh();
   }

   public bool ProgressStripe { get; set; }

   public void Progress()
   {
      value = index++;
      refresh();
   }

   public void Progress(int percentage)
   {
      _percentage = percentage;

      EmptyTextTitle = nil;
      Text = "";
      type = UiActionType.MuteProgress;
      Working = false;

      MessageShown?.Invoke(this, new MessageShownArgs("", type));

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.Value = _percentage | 0;
      }

      refresh();
   }

   public void StartStopwatch() => stopwatch.Value.Start();

   public void StopStopwatch() => stopwatch.Value.Stop();

   public void ResetStopwatch() => stopwatch.Value.Reset();

   public void Busy(string text)
   {
      Text = text;
      type = UiActionType.BusyText;
      Working = false;

      MessageShown?.Invoke(this, new MessageShownArgs(Text, type));

      if (TaskBarProgress && !_taskBarProgress)
      {
         _taskBarProgress = getTaskBarProgress();
      }

      this.Do(() => timerPaint.Enabled = true);
      refresh();
   }

   protected Color getForeColor(UiActionType type) => foreColors[type];

   protected Color getForeColor() => type == UiActionType.Display ? ForeColor : _foreColor | (() => foreColors[type]);

   protected Color getBackColor(UiActionType type) => backColors[type];

   protected Color getBackColor() => type == UiActionType.Display ? BackColor : _backColor | (() => backColors[type]);

   protected MessageStyle getStyle(UiActionType type) => styles[type];

   protected MessageStyle getStyle() => _style | (() => styles[type]);

   protected Rectangle getClientRectangle(Maybe<Rectangle> _labelRectangle)
   {
      if (Arrow)
      {
         var arrowSection = (int)(ClientRectangle.Width * START_AMOUNT);
         var remainder = ClientRectangle.Width - arrowSection;
         return ClientRectangle with { X = Width - arrowSection, Width = Width - 2 * remainder };
      }
      else if (_labelRectangle is (true, var labelRectangle))
      {
         return ClientRectangle with
         {
            X = ClientRectangle.X + labelRectangle.Width, Width = ClientRectangle.Width - labelRectangle.Width
         };
      }
      else
      {
         return ClientRectangle;
      }
   }

   protected Rectangle getRectangle()
   {
      if (_labelProcessor is (true, var labelProcessor))
      {
         return getClientRectangle(labelProcessor.Rectangle);
      }
      else
      {
         return getClientRectangle(nil);
      }
   }

   public bool ClickGlyph { get; set; }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      if (!Enabled)
      {
         var disabledWriter = new UiActionWriter(MessageAlignment, AutoSizeText, _floor, _ceiling)
         {
            Rectangle = ClientRectangle,
            Font = Font,
            Color = Color.Black,
            EmptyTextTitle = EmptyTextTitle,
            IsFile = IsFile
         };

         var filledRectangle = disabledWriter.TextRectangle(text, e.Graphics, ClientRectangle);
         fillRectangle(e.Graphics, Brushes.Gold, filledRectangle);
         drawRectangle(e.Graphics, Pens.Black, filledRectangle);

         disabledWriter.Write(text, e.Graphics);

         if (ProgressStripe && value < maximum)
         {
            var clientRectangleWidth = ClientRectangle.Width;
            var percentage = getPercentage(clientRectangleWidth);
            var top = ClientRectangle.Bottom - 4;
            var remainder = clientRectangleWidth - percentage;
            drawLine(e.Graphics, Color.Black, ((ClientRectangle.Left + percentage, top), (remainder, 0)));
         }

         return;
      }

      if (PaintOnRectangle is not null && rectangles.Length > 0)
      {
         for (var i = 0; i < rectangles.Length; i++)
         {
            PaintOnRectangle.Invoke(this, new UiActionRectanglePaintArgs(e.Graphics, i));
         }

         return;
      }

      activateProcessor(e.Graphics);
      if (_labelProcessor is (true, var labelProcessor))
      {
         labelProcessor.OnPaint(e.Graphics);
      }

      var clientRectangle = getRectangle();

      void paintStopwatch()
      {
         if (Stopwatch)
         {
            var elapsed = stopwatch.Value.Elapsed.ToString(@"mm\:ss");
            using var font = new Font("Consolas", 10);
            var size = TextRenderer.MeasureText(e.Graphics, elapsed, font);
            var location = new Point(clientRectangle.Width - size.Width - 8, 4);
            var rectangle = new Rectangle(location, size);
            if (StopwatchInverted)
            {
               var foreColor = getBackColor();
               var backColor = getForeColor();
               using var brush = new SolidBrush(backColor);
               e.Graphics.FillRectangle(brush, rectangle);
               TextRenderer.DrawText(e.Graphics, elapsed, font, rectangle, foreColor);
               using var pen = new Pen(foreColor);
               e.Graphics.DrawRectangle(pen, rectangle);
            }
            else
            {
               var foreColor = getForeColor();
               TextRenderer.DrawText(e.Graphics, elapsed, font, rectangle, foreColor);
               using var pen = new Pen(foreColor);
               e.Graphics.DrawRectangle(pen, rectangle);
            }
         }
      }

      var style = type switch
      {
         UiActionType.Busy or UiActionType.BusyText or UiActionType.ProgressDefinite or UiActionType.MuteProgress => CheckStyle.None,
         _ => CheckStyle
      };
      var writer = new Lazy<UiActionWriter>(() => new UiActionWriter(MessageAlignment, AutoSizeText, _floor, _ceiling)
      {
         Rectangle = clientRectangle,
         Font = getFont(),
         Color = getForeColor(),
         CheckStyle = style,
         EmptyTextTitle = EmptyTextTitle,
         IsFile = IsFile
      });
      var httpWriter = new Lazy<HttpWriter>(() => new HttpWriter(text, clientRectangle, getFont()));

      determineFloorAndCeiling();

      switch (type)
      {
         case UiActionType.ProgressIndefinite:
            writer.Value.Write(text, e.Graphics);
            break;
         case UiActionType.Busy when FlipFlop:
         {
            var foreColor = flipOn ? Color.White : Color.Black;
            var backColor = flipOn ? Color.Black : Color.White;
            var flipFlop = SubText("starting").Set.ForeColor(foreColor).BackColor(backColor).GoToUpperLeft(2).End;
            if (_flipFlop is (true, var oldFlipFlop))
            {
               RemoveSubText(oldFlipFlop);
               _flipFlop = flipFlop;
            }

            flipFlop.Draw(e.Graphics);

            flipOn = !flipOn;

            break;
         }
         case UiActionType.Busy when _busyProcessor is (true, var busyProcessor):
            busyProcessor.OnPaint(e.Graphics);
            break;
         case UiActionType.ProgressDefinite when _progressDefiniteProcessor is (true, var progressDefiniteProcessor):
         {
            var percentText = $"{getPercentage()}%";
            writer.Value.Rectangle = progressDefiniteProcessor.PercentRectangle;
            writer.Value.Center(true);
            writer.Value.Color = Color.Black;
            writer.Value.Write(percentText, e.Graphics);

            writer.Value.Rectangle = progressDefiniteProcessor.TextRectangle;
            writer.Value.Color = getForeColor();
            writer.Value.Write(text, e.Graphics);

            if (_progressSubText is (true, var progressSubText))
            {
               progressSubText.Draw(e.Graphics);
            }

            break;
         }
         case UiActionType.MuteProgress:
         {
            var percentText = $"{getPercentage()}%";
            writer.Value.Write(percentText, e.Graphics);

            if (_progressSubText is (true, var progressSubText))
            {
               progressSubText.Draw(e.Graphics);
            }

            break;
         }
         case UiActionType.BusyText when _busyTextProcessor is (true, var busyTextProcessor):
         {
            var allRectangle = writer.Value.TextRectangle(text, e.Graphics, clientRectangle);
            var allX = allRectangle.X;
            var allY = allRectangle.Y;
            var drawRectangle = busyTextProcessor.DrawRectangle;
            var drawX = drawRectangle.X + drawRectangle.Width;
            var drawY = drawRectangle.Y + drawRectangle.Height;
            if (allX < drawX || allY < drawY)
            {
               allRectangle = busyTextProcessor.TextRectangle;
            }

            writer.Value.Rectangle = allRectangle;
            writer.Value.Center(true);
            writer.Value.Write(text, e.Graphics);
            break;
         }
         case UiActionType.ControlLabel:
            writer.Value.Write(text, e.Graphics);
            break;
         case UiActionType.Http:
         {
            httpWriter.Value.OnPaint(e.Graphics, isUrlGood);
            break;
         }
         case UiActionType.Console:
            scroller.Value.OnPaint(e.Graphics);
            break;
         case UiActionType.Display:
            writer.Value.Color = ForeColor;
            writer.Value.Write(text, e.Graphics);
            break;
         default:
         {
            if (type != UiActionType.Tape)
            {
               writer.Value.Write(text, e.Graphics);
            }

            break;
         }
      }

      paintStopwatch();

      var clickGlyphWidth = 0;

      if (Clickable)
      {
         var color = getForeColor();
         drawClickGlyph(e, clientRectangle, color);

         clickGlyphWidth = 4;

         if (mouseInside || mouseDown)
         {
            using var dashedPen = new Pen(color, 1);
            dashedPen.DashStyle = DashStyle.Dash;
            var rectangle = clientRectangle;
            rectangle.Inflate(-2, -2);
            drawRectangle(e.Graphics, dashedPen, rectangle);
         }
      }

      if (ProgressStripe && value < maximum)
      {
         var clientRectangleWidth = clientRectangle.Width - clickGlyphWidth;
         var percentage = getPercentage(clientRectangleWidth);
         var top = clientRectangle.Bottom - 4;
         var color = getBackColor();
         drawLine(e.Graphics, color, ((clientRectangle.Left, top), (percentage, 0)));

         var remainder = clientRectangleWidth - percentage;
         color = getForeColor();
         drawLine(e.Graphics, color, ((clientRectangle.Left + percentage, top), (remainder, 0)));
      }

      if (ShowFocus && Focused)
      {
         var color = getForeColor();
         using var dashedPen = new Pen(color, 2);
         dashedPen.DashStyle = DashStyle.Dot;
         var rectangle = clientRectangle;
         rectangle.Inflate(-8, -8);
         drawRectangle(e.Graphics, dashedPen, rectangle);
      }

      drawAllSubTexts(e.Graphics, type, clientRectangle);

      drawClipToCancel(e, clientRectangle);

      Painting?.Invoke(this, e);
   }

   protected void drawClipToCancel(PaintEventArgs e, Rectangle clientRectangle)
   {
      if (clickToCancel)
      {
         using var font = new Font("Consolas", 9);
         var textSize = TextRenderer.MeasureText(e.Graphics, CLICK_TO_CANCEL, font);
         var x = clientRectangle.Width - textSize.Width - 8;
         var y = clientRectangle.Height - textSize.Height - 8;
         var textLocation = new Point(x, y);
         var textBounds = new Rectangle(textLocation, textSize);
         var foreColor = getBackColor();
         var backColor = getForeColor();
         using var brush = new SolidBrush(backColor);
         e.Graphics.FillRectangle(brush, textBounds);
         TextRenderer.DrawText(e.Graphics, CLICK_TO_CANCEL, font, textBounds, foreColor);
      }
   }

   protected void drawAllSubTexts(Graphics graphics, UiActionType type, Rectangle clientRectangle)
   {
      if (type is UiActionType.Busy or UiActionType.BusyText or UiActionType.ProgressDefinite or UiActionType.MuteProgress)
      {
         return;
      }

      var foreColor = new Lazy<Color>(getForeColor);
      var backColor = new Lazy<Color>(getBackColor);

      var _legend = legends.Peek();
      if (_legend is (true, var legend))
      {
         legend.Draw(graphics, foreColor.Value, backColor.Value);
      }

      if (Working && _working is (true, var working))
      {
         working.Draw(graphics, foreColor.Value, backColor.Value);
      }

      foreach (var subText in subTexts.Values)
      {
         subText.SetLocation(clientRectangle);
         subText.Draw(graphics, foreColor.Value, backColor.Value);
      }
   }

   private void drawClickGlyph(PaintEventArgs e, Rectangle clientRectangle, Color color)
   {
      if (ClickGlyph)
      {
         using var pen = new Pen(color, 4);
         if (Arrow)
         {
            var arrowSection = ClientRectangle.Width * START_AMOUNT;
            var arrowPoints = new PointF[]
            {
               new(arrowSection, 4),
               new(ClientRectangle.Width - 4, ClientRectangle.Height / 2.0f),
               new(arrowSection, ClientRectangle.Height - 4)
            };
            using var path = new GraphicsPath();
            path.AddLines(arrowPoints);

            e.Graphics.DrawPath(pen, path);
         }
         else
         {
            e.Graphics.DrawLine(pen, clientRectangle.Right - 4, 4, clientRectangle.Right - 4, clientRectangle.Bottom - 4);
         }
      }
   }

   protected override void OnPaintBackground(PaintEventArgs pevent)
   {
      if (!Enabled)
      {
         using var disabledBrush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
         fillRectangle(pevent.Graphics, disabledBrush, ClientRectangle);

         return;
      }

      base.OnPaintBackground(pevent);

      activateProcessor(pevent.Graphics);
      if (_labelProcessor is (true, var labelProcessor))
      {
         labelProcessor.OnPaintBackground(pevent.Graphics);
      }

      var clientRectangle = getRectangle();

      switch (type)
      {
         case UiActionType.Tape:
         {
            using var brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         case UiActionType.ProgressIndefinite or UiActionType.Busy:
         {
            using var brush = new SolidBrush(Color.DarkSlateGray);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         case UiActionType.ProgressDefinite when _progressDefiniteProcessor is (true, var progressDefiniteProcessor):
         {
            progressDefiniteProcessor.OnPaint(pevent.Graphics);
            var textRectangle = progressDefiniteProcessor.TextRectangle;

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
         case UiActionType.MuteProgress:
         {
            using var coralBrush = new SolidBrush(Color.Coral);
            fillRectangle(pevent.Graphics, coralBrush, clientRectangle);
            var width = clientRectangle.Width;
            var percentWidth = getPercentage(width);
            var location = clientRectangle.Location;
            var size = new Size(percentWidth, clientRectangle.Height);
            var rectangle = new Rectangle(location, size);
            using var cornflowerBlueBrush = new SolidBrush(Color.CornflowerBlue);
            fillRectangle(pevent.Graphics, cornflowerBlueBrush, rectangle);

            break;
         }
         case UiActionType.Unselected:
         {
            using var brush = new SolidBrush(Color.White);
            fillRectangle(pevent.Graphics, brush, clientRectangle);

            using var pen = new Pen(Color.DarkGray, 10);
            drawRectangle(pevent.Graphics, pen, clientRectangle);
            break;
         }
         case UiActionType.Selected:
         {
            using var brush = new SolidBrush(Color.White);
            fillRectangle(pevent.Graphics, brush, clientRectangle);

            using var pen = new Pen(Color.Black, 10);
            drawRectangle(pevent.Graphics, pen, clientRectangle);
            break;
         }
         case UiActionType.BusyText when _busyTextProcessor is (true, var busyTextProcessor):
         {
            using var brush = new SolidBrush(Color.Teal);
            fillRectangle(pevent.Graphics, brush, clientRectangle);

            busyTextProcessor.OnPaint(pevent);

            break;
         }
         case UiActionType.ControlLabel:
         {
            using var brush = new SolidBrush(Color.CadetBlue);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         case UiActionType.Http:
         {
            var httpWriter = new HttpWriter(text, clientRectangle, getFont());
            httpWriter.OnPaintBackground(pevent.Graphics, isUrlGood, mouseInside);
            break;
         }
         case UiActionType.Console:
            scroller.Value.OnPaintBackground(pevent.Graphics);
            break;
         case UiActionType.Display:
         {
            using var brush = new SolidBrush(BackColor);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         default:
         {
            var backColor = getBackColor();
            using var brush = new SolidBrush(backColor);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
      }

      if (isDirty)
      {
         var backColor = getBackColor();
         var foreColor = ControlPaint.Light(backColor);
         using var brush = new HatchBrush(HatchStyle.WideDownwardDiagonal, foreColor, backColor);
         fillRectangle(pevent.Graphics, brush, clientRectangle);
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

      int centerHorizontal(Image image)
      {
         var x = (clientRectangle.Width - image.Width) / 2;
         return x < 0 ? 2 : x;
      }

      int rightHorizontal(Image image)
      {
         var x = clientRectangle.Width - image.Width;
         return x < 0 ? 2 : x;
      }

      int centerVertical(Image image)
      {
         var y = (clientRectangle.Height - image.Height) / 2;
         return y < 0 ? 2 : y;
      }

      int bottomVertical(Image image)
      {
         var y = clientRectangle.Height - image.Height;
         return y < 0 ? 2 : y;
      }

      if (_image is (true, var image))
      {
         if (StretchImage)
         {
            pevent.Graphics.DrawImage(image, clientRectangle with { X = 0, Y = 0 });
         }
         else
         {
            var x = new Lazy<int>(() => centerHorizontal(image));
            var y = new Lazy<int>(() => centerVertical(image));
            var right = new Lazy<int>(() => rightHorizontal(image));
            var bottom = new Lazy<int>(() => bottomVertical(image));
            var location = CardinalAlignment switch
            {
               CardinalAlignment.Center => new Point(x.Value, y.Value),
               CardinalAlignment.North => new Point(x.Value, 2),
               CardinalAlignment.NorthEast => new Point(right.Value, 2),
               CardinalAlignment.East => new Point(right.Value, y.Value),
               CardinalAlignment.SouthEast => new Point(right.Value, bottom.Value),
               CardinalAlignment.South => new Point(x.Value, bottom.Value),
               CardinalAlignment.SouthWest => new Point(2, bottom.Value),
               CardinalAlignment.West => new Point(2, y.Value),
               CardinalAlignment.NorthWest => new Point(2, 2),
               _ => new Point(2, 2)
            };
            pevent.Graphics.DrawImage(image, location);
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

   protected int getPercentage() => _percentage | (() => (int)((float)value / maximum * 100));

   protected int getPercentage(int width)
   {
      return _percentage.Map(p => (int)(width * (p / 100.0))) | (() => (int)((float)value / maximum * width));
   }

   public int Index(bool increment) => increment ? index++ : index;

   public void Busy(bool enabled)
   {
      if (enabled)
      {
         Text = "";
         type = UiActionType.Busy;
         Working = false;
      }

      if (TaskBarProgress && !_taskBarProgress)
      {
         _taskBarProgress = getTaskBarProgress();
      }

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
         if (_lastType)
         {
            ShowMessage(text, _lastType);
            _lastType = nil;
         }
         else
         {
            ShowMessage(text, UiActionType.Uninitialized);
         }

         if (_lastEnabled is (true, true))
         {
            timerPaint.Enabled = true;
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

   protected virtual void drawLine(Graphics graphics, Color color, ((int x, int y), (int width, int height)) coordinates, float penWidth = 4,
      bool dashed = true)
   {
      var ((x, y), (width, height)) = coordinates;
      var x1 = x + width;
      var y1 = y + height;

      graphics.HighQuality();

      using var pen = new Pen(color, penWidth);
      if (dashed)
      {
         //pen.DashStyle = DashStyle.DashDotDot;
         //pen.DashCap = DashCap.Round;
         pen.DashPattern = new[] { 3.0f, 1.0f };
      }

      graphics.DrawLine(pen, x, y, x1, y1);
   }

   protected bool fillArrowRectangle(Graphics graphics, Brush brush, Rectangle rectangle)
   {
      if (Arrow)
      {
         graphics.HighQuality();
         var arrowSection = rectangle.Width * START_AMOUNT;
         var arrowPoints = new PointF[]
         {
            new(0, 0),
            new(arrowSection, 0),
            new(rectangle.Width, rectangle.Height / 2.0f),
            new(arrowSection, Height),
            new(0, rectangle.Height),
            new(rectangle.Width - arrowSection, rectangle.Height / 2.0f),
            new(0, 0)
         };

         using var path = new GraphicsPath();
         path.AddLines(arrowPoints);
         path.CloseFigure();

         graphics.FillPath(brush, path);

         return true;
      }
      else
      {
         return false;
      }
   }

   protected virtual void fillRectangle(Graphics graphics, Brush brush, Rectangle rectangle)
   {
      if (!fillArrowRectangle(graphics, brush, rectangle))
      {
         graphics.FillRectangle(brush, rectangle);
      }
   }

   protected void setFloor(int amount)
   {
      if (_floor is (true, var floor))
      {
         _floor = floor.MaxOf(amount);
      }
      else
      {
         _floor = amount;
      }
   }

   protected void setCeiling(int amount)
   {
      if (_ceiling is (true, var ceiling))
      {
         _ceiling = ceiling.MinOf(amount);
      }
      else
      {
         _ceiling = amount;
      }
   }

   protected void setFloorAndCeiling(int x, int y, Size size, bool includeFloor, bool includeCeiling)
   {
      setFloorAndCeiling(new Rectangle(new Point(x, y), size), includeFloor, includeCeiling);
   }

   protected void setFloorAndCeiling(int x, int y, int width, int height, bool includeFloor, bool includeCeiling)
   {
      setFloorAndCeiling(x, y, new Size(width, height), includeFloor, includeCeiling);
   }

   protected void setFloorAndCeiling(SubText subText)
   {
      setFloorAndCeiling(subText.X, subText.Y, subText.TextSize(nil).measuredSize, subText.IncludeFloor, subText.IncludeCeiling);
   }

   protected void setFloorAndCeiling(Rectangle rectangle, bool includeFloor, bool includeCeiling)
   {
      var halfway = ClientRectangle.Width / 2;
      var floor = rectangle.Left + rectangle.Width;
      var isLeft = floor < halfway;
      if (isLeft)
      {
         if (includeFloor)
         {
            setFloor(floor);
         }
      }
      else if (includeCeiling)
      {
         setCeiling(rectangle.Left);
      }
   }

   protected void determineFloorAndCeiling()
   {
      _floor = nil;
      _ceiling = nil;

      if (checkStyle is not CheckStyle.None)
      {
         setFloorAndCeiling(2, 2, 12, 12, true, true);
      }

      if (legends.Peek() is (true, var legend))
      {
         setFloorAndCeiling(legend);
      }

      foreach (var subText in subTexts.Values)
      {
         setFloorAndCeiling(subText);
      }

      if (Stopwatch)
      {
         var elapsed = stopwatch.Value.Elapsed.ToString(@"mm\:ss");
         using var font = new Font("Consolas", 10);
         var size = TextRenderer.MeasureText(elapsed, font, Size.Empty);
         var location = new Point(ClientRectangle.Width - size.Width - 8, 4);
         var rectangle = new Rectangle(location, size);
         setFloorAndCeiling(rectangle, true, true);
      }

      if (clickToCancel)
      {
         using var font = new Font("Consolas", 9);
         var textSize = TextRenderer.MeasureText(CLICK_TO_CANCEL, font, Size.Empty);
         var x = ClientRectangle.Width - textSize.Width - 8;
         var y = ClientRectangle.Height - textSize.Height - 8;
         var textLocation = new Point(x, y);
         var textBounds = new Rectangle(textLocation, textSize);
         setFloorAndCeiling(textBounds, true, true);
      }
   }

   public SubText SubText(SubText subText)
   {
      subTexts[subText.Id] = subText;
      determineFloorAndCeiling();

      return subText;
   }

   public SubText SubText(string text, int x, int y) => SubText(new SubText(text, x, y, ClientSize, ClickGlyph));

   public SubText SubText(string text) => SubText(text, 0, 0);

   public void RemoveSubText(Guid id)
   {
      subTexts.Remove(id);
      determineFloorAndCeiling();
      refresh();
   }

   public void RemoveSubText(SubText subText) => RemoveSubText(subText.Id);

   public void ClearSubTexts()
   {
      subTexts.Clear();
      determineFloorAndCeiling();
      refresh();
   }

   public void RunAsync()
   {
      var args = new ArgumentsArgs();
      Arguments?.Invoke(this, args);
      var _arguments = args.Arguments;
      if (_arguments is (true, var arguments))
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
      var args = new InitializeArgs();
      this.Do(() => Initialize?.Invoke(this, args));
      if (!args.Cancel && !backgroundWorker.Value.IsBusy)
      {
         var _argument = args.Argument;
         if (_argument is (true, var argument))
         {
            backgroundWorker.Value.RunWorkerAsync(argument);
         }
         else
         {
            backgroundWorker.Value.RunWorkerAsync();
         }
      }
   }

   public void RunWorkerAsync(object argument)
   {
      var args = new InitializeArgs { Argument = argument };
      this.Do(() => Initialize?.Invoke(this, args));
      if (!args.Cancel && !backgroundWorker.Value.IsBusy)
      {
         var _argument = args.Argument;
         if (_argument is (true, var argumentValue))
         {
            backgroundWorker.Value.RunWorkerAsync(argumentValue);
         }
         else
         {
            backgroundWorker.Value.RunWorkerAsync();
         }
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

   public void StartTimer(TimeSpan interval, bool oneTime = false)
   {
      oneTimeTimer = oneTime;
      timer.Interval = (int)interval.TotalMilliseconds;
      timer.Enabled = true;
   }

   public void StopTimer()
   {
      timer.Enabled = false;
      oneTimeTimer = false;
   }

   public void PerformClick() => OnClick(EventArgs.Empty);

   public bool TimerEnabled => timer.Enabled;

   protected (int, int) legendLocation()
   {
      if (_labelWidth is (true, var labelWidth))
      {
         return (labelWidth + 2, 2);
      }
      else if (_label is (true, var label))
      {
         var width = LabelProcessor.LabelWidth(label, getFont()) | 2;
         return (width + 2, 2);
      }
      else if (CheckStyle != CheckStyle.None)
      {
         return (20, 2);
      }
      else
      {
         return (2, 2);
      }
   }

   public SubText Legend(string text, bool invert = true)
   {
      var (x, y) = legendLocation();
      var legend = new SubText(text, x, y, ClientSize, true)
         .Set
         .FontSize(8)
         .Outline()
         .Invert(invert)
         .End;
      legends.Push(legend);

      determineFloorAndCeiling();

      return legend;
   }

   public SubText Legend(string text, int x, int y, bool invert = true)
   {
      var legend = new SubText(text, x, y, ClientSize, true)
         .Set
         .FontSize(8)
         .Outline()
         .Invert(invert)
         .End;
      legends.Push(legend);

      determineFloorAndCeiling();

      return legend;
   }

   public async Task LegendAsync(string text, TimeSpan delay, bool invert = true)
   {
      Legend(text, invert);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task LegendAsync(string text, bool invert = true) => await LegendAsync(text, 2.Seconds(), invert);

   public async Task LegendAsync(string text, int x, int y, TimeSpan delay, bool invert = true)
   {
      Legend(text, x, y, invert);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task LegendAsync(string text, int x, int y, bool invert = true)
   {
      await LegendAsync(text, x, y, 2.Seconds(), invert);
   }

   public void LegendTemp(string text, TimeSpan delay, bool invert = true)
   {
      this.Do(() => Task.Run(() => LegendAsync(text, delay, invert)));
   }

   public void LegendTemp(string text, bool invert = true) => LegendTemp(text, 2.Seconds(), invert);

   public void LegendTemp(string text, int x, int y, TimeSpan delay, bool invert = true)
   {
      this.Do(() => Task.Run(() => LegendAsync(text, x, y, delay, invert)));
   }

   public void LegendTemp(string text, int x, int y, bool invert = true) => LegendTemp(text, x, y, 2.Seconds(), invert);

   public SubText SuccessLegend(string text)
   {
      return Legend(text, false).Set.ForeColor(Color.White).BackColor(Color.Green).End;
   }

   public SubText SuccessLegend(string text, int x, int y)
   {
      return Legend(text, x, y, false).Set.ForeColor(Color.White).BackColor(Color.Green).End;
   }

   public async Task SuccessLegendAsync(string text, TimeSpan delay)
   {
      SuccessLegend(text);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task SuccessLegendAsync(string text) => await SuccessLegendAsync(text, 2.Seconds());

   public async Task SuccessLegendAsync(string text, int x, int y, TimeSpan delay)
   {
      SuccessLegend(text, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task SuccessLegendAsync(string text, int x, int y) => await SuccessLegendAsync(text, x, y, 2.Seconds());

   public void SuccessLegendTemp(string text, TimeSpan delay) => this.Do(() => Task.Run(() => SuccessLegendAsync(text, delay)));

   public void SuccessLegendTemp(string text) => SuccessLegendTemp(text, 2.Seconds());

   public void SuccessLegendTemp(string text, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => SuccessLegendAsync(text, x, y, delay)));
   }

   public void SuccessLegendTemp(string text, int x, int y) => SuccessLegendTemp(text, x, y, 2.Seconds());

   public SubText FailureLegend(string text)
   {
      return Legend(text, false).Set.ForeColor(Color.Black).BackColor(Color.Gold).End;
   }

   public SubText FailureLegend(string text, int x, int y)
   {
      return Legend(text, x, y, false).Set.ForeColor(Color.Black).BackColor(Color.Gold).End;
   }

   public async Task FailureLegendAsync(string text, TimeSpan delay)
   {
      FailureLegend(text);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task FailureLegendAsync(string text) => await FailureLegendAsync(text, 2.Seconds());

   public async Task FailureLegendAsync(string text, int x, int y, TimeSpan delay)
   {
      FailureLegend(text, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task FailureLegendAsync(string text, int x, int y) => await FailureLegendAsync(text, x, y, 2.Seconds());

   public void FailureLegendTemp(string text, TimeSpan delay) => this.Do(() => Task.Run(() => FailureLegendAsync(text, delay)));

   public void FailureLegendTemp(string text) => FailureLegendTemp(text, 2.Seconds());

   public void FailureLegendTemp(string text, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => FailureLegendAsync(text, x, y, delay)));
   }

   public void FailureLegendTemp(string text, int x, int y) => FailureLegendTemp(text, x, y, 2.Seconds());

   public SubText ExceptionLegend(Exception exception)
   {
      return Legend(exception.Message, false).Set.ForeColor(Color.White).BackColor(Color.Red).End;
   }

   public SubText ExceptionLegend(Exception exception, int x, int y)
   {
      return Legend(exception.Message, x, y, false).Set.ForeColor(Color.White).BackColor(Color.Red).End;
   }

   public async Task ExceptionLegendSync(Exception exception, TimeSpan delay)
   {
      ExceptionLegend(exception);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ExceptionLegendSync(Exception exception) => await ExceptionLegendSync(exception, 2.Seconds());

   public async Task ExceptionLegendSync(Exception exception, int x, int y, TimeSpan delay)
   {
      ExceptionLegend(exception, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ExceptionLegendSync(Exception exception, int x, int y) => await ExceptionLegendSync(exception, x, y, 2.Seconds());

   public void ExceptionLegendTemp(Exception exception, TimeSpan delay) => this.Do(() => Task.Run(() => ExceptionLegendSync(exception, delay)));

   public void ExceptionLegendTemp(Exception exception) => ExceptionLegendTemp(exception, 2.Seconds());

   public void ExceptionLegendTemp(Exception exception, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => ExceptionLegendSync(exception, x, y, delay)));
   }

   public void ExceptionLegendTemp(Exception exception, int x, int y) => ExceptionLegendTemp(exception, x, y, 2.Seconds());

   public SubText ResultLegend(Result<string> _result)
   {
      if (_result is (true, var result))
      {
         return SuccessLegend(result);
      }
      else
      {
         return ExceptionLegend(_result.Exception);
      }
   }

   public SubText ResultLegend(Result<string> _result, int x, int y)
   {
      if (_result is (true, var result))
      {
         return SuccessLegend(result, x, y);
      }
      else
      {
         return ExceptionLegend(_result.Exception, x, y);
      }
   }

   public async Task ResultLegendAsync(Result<string> _result, TimeSpan delay)
   {
      ResultLegend(_result);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync(Result<string> _result) => await ResultLegendAsync(_result, 2.Seconds());

   public async Task ResultLegendAsync(Result<string> _result, int x, int y, TimeSpan delay)
   {
      ResultLegend(_result, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync(Result<string> _result, int x, int y) => await ResultLegendAsync(_result, x, y, 2.Seconds());

   public void ResultLegendTemp(Result<string> _result, TimeSpan delay) => this.Do(() => Task.Run(() => ResultLegendAsync(_result, delay)));

   public void ResultLegendTemp(Result<string> _result) => ResultLegendTemp(_result, 2.Seconds());

   public void ResultLegendTemp(Result<string> _result, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => ResultLegendAsync(_result, x, y, delay)));
   }

   public void ResultLegendTemp(Result<string> _result, int x, int y) => ResultLegendTemp(_result, x, y, 2.Seconds());

   public SubText ResultLegend((string, UiActionType) result)
   {
      var (message, uiActionType) = result;
      return uiActionType switch
      {
         UiActionType.Success => SuccessLegend(message),
         UiActionType.Failure => FailureLegend(message),
         UiActionType.Exception => ExceptionLegend(fail(message)),
         _ => Legend(message)
      };
   }

   public SubText ResultLegend((string, UiActionType) result, int x, int y)
   {
      var (message, uiActionType) = result;
      return uiActionType switch
      {
         UiActionType.Success => SuccessLegend(message, x, y),
         UiActionType.Failure => FailureLegend(message, x, y),
         UiActionType.Exception => ExceptionLegend(fail(message), x, y),
         _ => Legend(message, x, y)
      };
   }

   public async Task ResultLegendAsync((string, UiActionType) result, TimeSpan delay)
   {
      ResultLegend(result);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync((string, UiActionType) result) => await ResultLegendAsync(result, 2.Seconds());

   public async Task ResultLegendAsync((string, UiActionType) result, int x, int y, TimeSpan delay)
   {
      ResultLegend(result, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync((string, UiActionType) result, int x, int y) => await ResultLegendAsync(result, x, y, 2.Seconds());

   public void ResultLegendTemp((string, UiActionType) result, TimeSpan delay) => this.Do(() => Task.Run(() => ResultLegendAsync(result, delay)));

   public void ResultLegendTemp((string, UiActionType) result) => ResultLegendTemp(result, 2.Seconds());

   public void ResultLegendTemp((string, UiActionType) result, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => ResultLegendAsync(result, x, y, delay)));
   }

   public void ResultLegendTemp((string, UiActionType) result, int x, int y) => ResultLegendTemp(result, x, y, 2.Seconds());

   public SubText Notify(string text) => Legend(text, false).Set.ForeColor(Color.Black).BackColor(Color.White).End;

   public SubText Notify(string text, int x, int y) => Legend(text, x, y, false).Set.ForeColor(Color.Black).BackColor(Color.White).End;

   public async Task NotifyAsync(string text, TimeSpan delay)
   {
      Notify(text);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task NotifyAsync(string text) => await NotifyAsync(text, 2.Seconds());

   public async Task NotifyAsync(string text, int x, int y, TimeSpan delay)
   {
      Notify(text, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task NotifyAsync(string text, int x, int y) => await NotifyAsync(text, x, y, 2.Seconds());

   public void NotifyTemp(string text, TimeSpan delay) => this.Do(() => Task.Run(() => NotifyAsync(text, delay)));

   public void NotifyTemp(string text) => NotifyTemp(text, 2.Seconds());

   public void NotifyTemp(string text, int x, int y, TimeSpan delay) => this.Do(() => Task.Run(() => NotifyAsync(text, x, y, delay)));

   public void NotifyTemp(string text, int x, int y) => NotifyTemp(text, x, y, 2.Seconds());

   public void Legend()
   {
      legends.Pop();
      refresh();
   }

   public void ClearAllLegends()
   {
      legends.Clear();
      refresh();
   }

   public bool Working
   {
      get => workingTimer.Enabled;
      set
      {
         workingTimer.Enabled = value;
         _working = nil;
      }
   }

   public void Validate(string text)
   {
      var args = new ValidatedArgs(text);
      ValidateText?.Invoke(this, args);
      ShowMessage(text, args.Type);
   }

   public bool ShowFocus { get; set; }

   public Maybe<string> EmptyTextTitle { get; set; }

   public async Task<Completion<TResult>> ExecuteAsync<TArgument, TResult>(TArgument argument, Func<TArgument, Completion<TResult>> func,
      Action<TResult> postAction)
   {
      var _result = await ExecuteAsync(argument, func);
      if (_result is (true, var result))
      {
         postAction(result);
      }

      return _result;
   }

   public async Task<Completion<TResult>> ExecuteAsync<TArgument, TResult>(TArgument argument, Func<TArgument, Completion<TResult>> func)
   {
      return await Task.Run(() => func(argument));
   }

   public async Task<Completion<Unit>> ExecuteAsync<TArgument>(TArgument argument, Func<TArgument, Completion<Unit>> func, Action postAction)
   {
      var _result = await ExecuteAsync(argument, func);
      if (_result)
      {
         postAction();
      }

      return _result;
   }

   public async Task<Completion<Unit>> ExecuteAsync<TArgument>(TArgument argument, Func<TArgument, Completion<Unit>> func)
   {
      return await Task.Run(() => func(argument));
   }

   public async Task<Completion<TResult>> ExecuteAsync<TResult>(Func<Completion<TResult>> func, Action<TResult> postAction)
   {
      var _result = await ExecuteAsync(func);
      if (_result is (true, var result))
      {
         postAction(result);
      }

      return _result;
   }

   public async Task<Completion<TResult>> ExecuteAsync<TResult>(Func<Completion<TResult>> func) => await Task.Run(func);

   public async Task<Completion<Unit>> ExecuteAsync(Func<Completion<Unit>> func, Action postAction)
   {
      var _result = await ExecuteAsync(func);
      if (_result)
      {
         postAction();
      }

      return _result;
   }

   public async Task<Completion<Unit>> ExecuteAsync(Func<Completion<Unit>> func) => await Task.Run(func);

   public bool IsDirty
   {
      get => isDirty;
      set
      {
         isDirty = value;
         refresh();
      }
   }

   public ChooserSet Choose(string title, int width)
   {
      var chooser = new Chooser(title, this, width);
      if (AppearanceOverride is not null)
      {
         chooser.AppearanceOverride += (sender, args) => AppearanceOverride.Invoke(sender, args);
      }

      return new ChooserSet(chooser);
   }

   public ChooserSet Choose(string title)
   {
      var chooser = new Chooser(title, this, nil);
      if (AppearanceOverride is not null)
      {
         chooser.AppearanceOverride += (sender, args) => AppearanceOverride.Invoke(sender, args);
      }

      return new ChooserSet(chooser);
   }

   public bool Arrow { get; set; }

   public void ControlLabel(string text) => ShowMessage(text, UiActionType.ControlLabel);

   public LabelSetter Label(string label) => new LabelSetter(this).Label(label);

   public LabelSetter Label() => new LabelSetter(this).Label();

   public void Http(string url)
   {
      isUrlGood = HttpWriter.IsGoodUrl(url);
      ShowMessage(url, UiActionType.Http);
   }

   protected void openUrl(object sender, EventArgs e)
   {
      if (text.IsNotEmpty())
      {
         using var process = new Process();
         process.StartInfo.UseShellExecute = true;
         process.StartInfo.FileName = text;
         process.Start();
      }
   }

   public void SizeToText()
   {
      var size = TextRenderer.MeasureText(text, Font);
      Width = size.Width + 40;
   }

   public void WriteLine(object obj)
   {
      Type = UiActionType.Console;
      scroller.Value.WriteLine(obj);

      MessageShown?.Invoke(this, new MessageShownArgs(Text, type));

      refresh();
   }

   public void FloatingFailure(string message)
   {
      FloatingFailure(false);

      _failureToolTip = message;
      _failureSubText = SubText("failure").Set.GoToUpperLeft(0).Font("Consolas", 8).ForeColor(Color.Black).BackColor(Color.Gold).End;

      setToolTip();
   }

   public void FloatingFailure(bool set = true)
   {
      _failureToolTip = nil;
      if (_failureSubText is (true, var failureSubText))
      {
         subTexts.Remove(failureSubText.Id);
      }

      _failureSubText = nil;

      if (set)
      {
         setToolTip();
      }
   }

   public void FloatingNoStatus(string message)
   {
      FloatingNoStatus(false);

      _noStatusToolTip = message;
      _noStatusSubText = SubText("no status").Set.GoToUpperLeft(0).Font("Consolas", 8).ForeColor(Color.Black).BackColor(Color.White).End;
   }

   public void FloatingNoStatus(bool set = true)
   {
      _noStatusToolTip = nil;
      if (_noStatusSubText is (true, var noStatusSubText))
      {
         subTexts.Remove(noStatusSubText.Id);
      }

      _noStatusSubText = nil;

      if (set)
      {
         setToolTip();
      }
   }

   public Maybe<string> FailureToolTip => _failureToolTip;

   public Maybe<string> NoStatusToolTip => _noStatusToolTip;

   public void FloatingException(Exception exception)
   {
      FloatingException(false);

      _exceptionToolTip = exception.Message;
      _exceptionSubText = SubText("exception").Set.GoToUpperLeft(0).Font("Consolas", 8).ForeColor(Color.White).BackColor(Color.Red).End;

      setToolTip();
   }

   public void FloatingException(bool set = true)
   {
      _exceptionToolTip = nil;
      if (_exceptionSubText is (true, var exceptionSubText))
      {
         subTexts.Remove(exceptionSubText.Id);
      }

      _exceptionSubText = nil;

      if (set)
      {
         setToolTip();
      }
   }

   public bool FlipFlop { get; set; }

   public Maybe<string> ExceptionToolTip => _exceptionToolTip;

   public bool HasFloatingFailureOrException => _failureToolTip || _exceptionToolTip;

   public void ClearFloating()
   {
      FloatingFailure(false);
      FloatingException(false);
      FloatingNoStatus(false);

      setToolTip();
   }

   public string ToolTipTitle
   {
      get => toolTip.ToolTipTitle;
      set
      {
         toolTip.ToolTipTitle = value;
         setToolTip();
      }
   }

   public bool ToolTipBox
   {
      get => toolTip.ToolTipBox;
      set
      {
         toolTip.ToolTipBox = value;
         setToolTip();
      }
   }

   public Maybe<SubText> ProgressSubText
   {
      get => _progressSubText;
      set
      {
         if (_progressSubText is (true, var progressSubText))
         {
            RemoveSubText(progressSubText);
         }

         _progressSubText = value;
      }
   }

   public bool IsFailureOrException => type is UiActionType.Failure or UiActionType.Exception;

   public bool IsABusyType
   {
      get
      {
         return type is UiActionType.Busy or UiActionType.BusyText or UiActionType.ProgressDefinite or UiActionType.ProgressIndefinite
            or UiActionType.MuteProgress;
      }
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      foreach (var (_, subText) in subTexts)
      {
         var clientRectangle = getClientRectangle(nil);
         subText.SetLocation(clientRectangle);
      }
   }

   public int RectangleCount
   {
      get => rectangles.Length;
      set
      {
         if (value > 0)
         {
            var clientWidth = ClientRectangle.Width;
            var clientHeight = ClientRectangle.Height;

            var paddingWidth = 2;
            var width = (clientWidth - (value + 1) * paddingWidth) / value;
            var top = paddingWidth;
            var height = clientHeight - 2 * paddingWidth;
            var fullWidth = paddingWidth + width;

            rectangles = Enumerable.Range(0, value).Select(i => new Rectangle(i * fullWidth, top, width, height)).ToArray();
         }
         else
         {
            rectangles = Array.Empty<Rectangle>();
         }
      }
   }

   public Rectangle[] Rectangles => rectangles;
}