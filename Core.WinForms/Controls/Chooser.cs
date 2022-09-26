using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class Chooser : Form
{
   private const int WM_NCACTIVATE = 0x86;

   public Maybe<Chosen> Get()
   {
      ShowDialog();
      return Choice;
   }

   protected string title;
   protected UiAction uiAction;
   protected StringHash choices;
   protected Maybe<Color> _foreColor;
   protected Maybe<Color> _backColor;
   protected Maybe<string> _nilItem;
   protected bool modifyTitle;
   protected string emptyTitle;

   public event EventHandler<AppearanceOverrideArgs> AppearanceOverride;

   public Chooser(string title, UiAction uiAction, Maybe<int> _width)
   {
      this.title = title;
      this.uiAction = uiAction;

      choices = new StringHash(true);
      _foreColor = nil;
      _backColor = nil;
      _nilItem = "none";
      modifyTitle = true;
      emptyTitle = "";

      InitializeComponent();

      Choice = nil;

      if (_width)
      {
         Width = _width;
      }
   }

   public UiAction UiAction => uiAction;

   public ChooserSet Set => new(this);

   public string Title
   {
      get => title;
      set => title = value;
   }

   public StringHash Choices
   {
      get => choices;
      set => choices = value;
   }

   public Color ChoiceForeColor
   {
      get => _foreColor | Color.Black;
      set => _foreColor = value;
   }

   public Color ChoiceBackColor
   {
      get => _backColor | Color.Gold;
      set => _backColor = value;
   }

   public Maybe<string> NilItem
   {
      get => _nilItem;
      set => _nilItem = value;
   }

   public bool ModifyTitle
   {
      get => modifyTitle;
      set => modifyTitle = value;
   }

   public string EmptyTitle
   {
      get => emptyTitle;
      set => emptyTitle = value;
   }

   public Maybe<Chosen> Choice { get; set; }

   protected void addItem(string text, Color foreColor, Color backColor)
   {
      Maybe<Font> _font = nil;

      if (AppearanceOverride is not null)
      {
         var args = new AppearanceOverrideArgs(text, foreColor, backColor);
         AppearanceOverride.Invoke(this, args);
         if (args.Override)
         {
            text = args.Text;
            foreColor = args.ForeColor;
            backColor = args.BackColor;

            Bits32<FontStyle> style = FontStyle.Regular;
            var modified = false;
            if (args.Italic)
            {
               style[FontStyle.Italic] = true;
               modified = true;
            }

            if (args.Bold)
            {
               style[FontStyle.Bold] = true;
               modified = true;
            }

            _font = maybe<Font>() & modified & (() => new Font(listViewItems.Font, style));
         }
      }

      var item = listViewItems.Items.Add(text);
      item.UseItemStyleForSubItems = true;
      item.ForeColor = foreColor;
      item.BackColor = backColor;

      if (_font)
      {
         item.Font = _font;
      }
   }

   protected void Chooser_Load(object sender, EventArgs e)
   {
      Location = Cursor.Position;
      if (_nilItem)
      {
         addItem(_nilItem, _foreColor | Color.White, _backColor | Color.Blue);
      }

      if (!_foreColor)
      {
         _foreColor = Color.White;
      }

      if (!_backColor)
      {
         _backColor = Color.Green;
      }

      foreach (var choice in choices.Keys)
      {
         addItem(choice, _foreColor, _backColor);
      }

      listViewItems.Columns[0].Width = ClientSize.Width;
      listViewItems.Columns[0].Text = title;

      var lastItem = listViewItems.Items[listViewItems.Items.Count - 1];
      var bounds = lastItem.Bounds;
      var bottom = bounds.Bottom;
      Height = bottom + 4;
   }

   protected void Chooser_MouseDown(object sender, MouseEventArgs e)
   {
      if (!ClientRectangle.Contains(Cursor.Position))
      {
         Close();
      }
   }

   protected bool returnSome(int index) => _nilItem.Map(_ => index > 0) | (() => index > -1);

   protected Maybe<Chosen> getChosen(ListViewItem item)
   {
      return choices.Map(item.Text).Map(value => new Chosen(value, item));
   }

   protected void listViewItems_SelectedIndexChanged(object sender, EventArgs e)
   {
      Choice = listViewItems.SelectedItem().Map(item => maybe<Chosen>() & returnSome(item.Index) & (() => getChosen(item)));
      Close();
   }

   protected override void WndProc(ref Message m)
   {
      base.WndProc(ref m);

      if (m.Msg == WM_NCACTIVATE && Visible && !RectangleToScreen(DisplayRectangle).Contains(Cursor.Position))
      {
         modifyTitle = false;
         Close();
      }
   }
}