﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class Chooser : Form
{
   protected const int WM_NCACTIVATE = 0x86;

   public Optional<Chosen> Get(UiAction parent)
   {
      //ShowDialog(parent);
      ShowDialog();
      return Choice;
   }

   protected string title;
   protected UiAction uiAction;
   protected Point uiLocation;
   protected StringHash choices;
   protected Optional<Color> _foreColor;
   protected Optional<Color> _backColor;
   protected Optional<string> _nilItem;
   protected bool modifyTitle;
   protected string emptyTitle;
   protected bool sizeToText;

   public event EventHandler<AppearanceOverrideArgs> AppearanceOverride;

   public Chooser(string title, UiAction uiAction, Optional<int> _width)
   {
      this.title = title;
      this.uiAction = uiAction;
      uiLocation = this.uiAction.Location;

      choices = new StringHash(true);
      _foreColor = nil;
      _backColor = nil;
      _nilItem = "none";
      modifyTitle = true;
      emptyTitle = "";
      sizeToText = false;

      InitializeComponent();

      Choice = nil;

      AutoScaleMode = AutoScaleMode.Inherit;

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

   public Optional<string> NilItem
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

   public bool SizeToText
   {
      get => sizeToText;
      set => sizeToText = value;
   }

   public Optional<Chosen> Choice { get; set; }

   protected void addItem(string text, Color foreColor, Color backColor)
   {
      Optional<Font> _font = nil;

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

   protected void locate()
   {
      var screen = Screen.GetWorkingArea(this);
      var size = Size;
      Console.WriteLine($"size: {size}");
      var location = Cursor.Position;

      var right = location.X + size.Width;
      var xDifference = screen.Right - right;
      if (xDifference < 0)
      {
         location.X += xDifference;
      }

      var bottom = location.Y + size.Height / 3;
      var yDifference = screen.Bottom - bottom;
      if (yDifference < 0)
      {
         location.Y += yDifference;
      }

      Location = location;
   }

   protected void Chooser_Load(object sender, EventArgs e)
   {
      locate();
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

   protected Optional<Chosen> getChosen(ListViewItem item)
   {
      return choices.Maybe(item.Text).Map(value => new Chosen(value, item));
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