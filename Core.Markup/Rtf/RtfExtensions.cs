﻿namespace Core.Markup.Rtf;

public static class RtfExtensions
{
   public static Hyperlink Link(this string link, string linkTip) => new(link, linkTip);

   public static Hyperlink Link(this string link) => new(link);

   public static FirstLineIndent FirstLineIndent(this float amount) => new(amount);

   public static FirstLineIndent FirstLineIndent(this int amount) => new(amount);

   public static Bookmark Bookmark(this string name) => new(name);
}