using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Notification
{
	public class Notifier : Component
	{
		const int DEFAULT_DURATION = 10000;
		IContainer components;

		public Notifier() => components = new Container();

		public Notifier(IContainer container)
		{
			container.Add(this);
			components = new Container();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
         {
            components?.Dispose();
         }

         base.Dispose(disposing);
		}

		[Description("Left side color"), Category("Appearance"), DefaultValue(typeof(ColorDialog), null), Browsable(true)]
		public Color LeftColor { get; set; } = Color.FromArgb(150, 150, 150);

		[Description("Right side color"), Category("Appearance"), DefaultValue(typeof(ColorDialog), null), Browsable(true)]
		public Color RightColor { get; set; } = Color.FromArgb(204, 204, 204);

		[Description("Duration to display"), Category("Appearance"), DefaultValue(typeof(NumericUpDown), null), Browsable(true)]
		public int Duration { get; set; } = 3000;

		[Description("Icon on left side"), Category("Appearance"), DefaultValue(typeof(PictureBox), null), Browsable(true)]
		public Image Icon { get; set; }

		[Description("Text"), Category("Appearance"), DefaultValue(typeof(TextBox), null), Browsable(true)]
		public string Text { get; set; } = "";

		[Description("Color of text"), Category("Appearance"), DefaultValue(typeof(ColorDialog), null), Browsable(true)]
		public Color TextColor { get; set; } = Color.Black;

		[Description("Title"), Category("Appearance"), DefaultValue(typeof(TextBox), null), Browsable(true)]
		public string Title { get; set; } = "";

		[Description("Color of title"), Category("Appearance"), DefaultValue(typeof(ColorDialog), null), Browsable(true)]
		public Color TitleColor { get; set; } = Color.Black;

		public static void ShowNotifier(string title, string text, int duration = 3000)
		{
			showNotififer(duration, title, text, Color.FromArgb(150, 150, 150), Color.FromArgb(204, 204, 204), Color.Black, Color.Black,
				"information");
		}

		public static void ShowNotifier(string title, string text, Image icon, string leftColor, string rightColor, string titleColor = "Black",
			string textColor = "Black", int duration = DEFAULT_DURATION)
		{
			var actualLeftColor = Color.FromName(leftColor);
			var actualRightColor = Color.FromName(rightColor);
			var actualTitleColor = Color.FromName(titleColor);
			var actualTextColor = Color.FromName(textColor);

			showNotififer(duration, title, text, actualLeftColor, actualRightColor, actualTitleColor, actualTextColor, icon);
		}

		public static void ShowNotifier(string title, string text, string icon, string leftColor, string rightColor, string titleColor = "Black",
			string textColor = "Black", int duration = DEFAULT_DURATION)
		{
			var actualLeftColor = Color.FromName(leftColor);
			var actualRightColor = Color.FromName(rightColor);
			var actualTitleColor = Color.FromName(titleColor);
			var actualTextColor = Color.FromName(textColor);

			showNotififer(duration, title, text, actualLeftColor, actualRightColor, actualTitleColor, actualTextColor, icon);
		}

      static void showNotififer(int duration, string title, string text, Color leftColor, Color rightColor, Color titleColor,
			Color textColor, object icon)
		{
			var host = new NotifierHost(duration, title, text, icon, leftColor, rightColor, titleColor, textColor);
			host.Show();
		}

		public static void ShowError(string title, string text, int duration = DEFAULT_DURATION)
		{
			ShowNotifier(title, text, "error", "MistyRose", "Red", "White", "White", duration);
		}

		public static void ShowInfo(string title, string text, int duration = DEFAULT_DURATION)
		{
			ShowNotifier(title, text, "info", "PaleTurquoise", "Turquoise", duration: duration);
		}

		public static void ShowWarning(string title, string text, int duration = DEFAULT_DURATION)
		{
			ShowNotifier(title, text, "warning", "Bisque", "Yellow", duration: duration);
		}
	}
}