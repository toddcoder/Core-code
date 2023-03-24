using Core.Matching;
using System.Collections.Generic;
using System.Globalization;
using Core.Collections;
using Core.DataStructures;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Markup.Rtf;

public static class RtfStripperFunction
{
   private class StackEntry
   {
      public StackEntry(int numberOfCharactersToSkip, bool ignorable)
      {
         NumberOfCharactersToSkip = numberOfCharactersToSkip;
         Ignorable = ignorable;
      }

      public int NumberOfCharactersToSkip { get; }

      public bool Ignorable { get; }
   }

   private static readonly Pattern pattern = @"\\([a-z]{1,32})(-?\d{1,10})?[ ]?|\\'([0-9a-f]{2})|\\([^a-z])|([{}])|[\r\n]+|(.); usi";

   private static readonly List<string> destinations = new()
   {
      "aftncn", "aftnsep", "aftnsepc", "annotation", "atnauthor", "atndate", "atnicn", "atnid",
      "atnparent", "atnref", "atntime", "atrfend", "atrfstart", "author", "background",
      "bkmkend", "bkmkstart", "blipuid", "buptim", "category", "colorschememapping",
      "colortbl", "comment", "company", "creatim", "datafield", "datastore", "defchp", "defpap",
      "do", "doccomm", "docvar", "dptxbxtext", "ebcend", "ebcstart", "factoidname", "falt",
      "fchars", "ffdeftext", "ffentrymcr", "ffexitmcr", "ffformat", "ffhelptext", "ffl",
      "ffname", "ffstattext", "field", "file", "filetbl", "fldinst", "fldrslt", "fldtype",
      "fname", "fontemb", "fontfile", "fonttbl", "footer", "footerf", "footerl", "footerr",
      "footnote", "formfield", "ftncn", "ftnsep", "ftnsepc", "g", "generator", "gridtbl",
      "header", "headerf", "headerl", "headerr", "hl", "hlfr", "hlinkbase", "hlloc", "hlsrc",
      "hsv", "htmltag", "info", "keycode", "keywords", "latentstyles", "lchars", "levelnumbers",
      "leveltext", "lfolevel", "linkval", "list", "listlevel", "listname", "listoverride",
      "listoverridetable", "listpicture", "liststylename", "listtable", "listtext",
      "lsdlockedexcept", "macc", "maccPr", "mailmerge", "maln", "malnScr", "manager", "margPr",
      "mbar", "mbarPr", "mbaseJc", "mbegChr", "mborderBox", "mborderBoxPr", "mbox", "mboxPr",
      "mchr", "mcount", "mctrlPr", "md", "mdeg", "mdegHide", "mden", "mdiff", "mdPr", "me",
      "mendChr", "meqArr", "meqArrPr", "mf", "mfName", "mfPr", "mfunc", "mfuncPr", "mgroupChr",
      "mgroupChrPr", "mgrow", "mhideBot", "mhideLeft", "mhideRight", "mhideTop", "mhtmltag",
      "mlim", "mlimloc", "mlimlow", "mlimlowPr", "mlimupp", "mlimuppPr", "mm", "mmaddfieldname",
      "mmath", "mmathPict", "mmathPr", "mmaxdist", "mmc", "mmcJc", "mmconnectstr",
      "mmconnectstrdata", "mmcPr", "mmcs", "mmdatasource", "mmheadersource", "mmmailsubject",
      "mmodso", "mmodsofilter", "mmodsofldmpdata", "mmodsomappedname", "mmodsoname",
      "mmodsorecipdata", "mmodsosort", "mmodsosrc", "mmodsotable", "mmodsoudl",
      "mmodsoudldata", "mmodsouniquetag", "mmPr", "mmquery", "mmr", "mnary", "mnaryPr",
      "mnoBreak", "mnum", "mobjDist", "moMath", "moMathPara", "moMathParaPr", "mopEmu",
      "mphant", "mphantPr", "mplcHide", "mpos", "mr", "mrad", "mradPr", "mrPr", "msepChr",
      "mshow", "mshp", "msPre", "msPrePr", "msSub", "msSubPr", "msSubSup", "msSubSupPr", "msSup",
      "msSupPr", "mstrikeBLTR", "mstrikeH", "mstrikeTLBR", "mstrikeV", "msub", "msubHide",
      "msup", "msupHide", "mtransp", "mtype", "mvertJc", "mvfmf", "mvfml", "mvtof", "mvtol",
      "mzeroAsc", "mzeroDesc", "mzeroWid", "nesttableprops", "nextfile", "nonesttables",
      "objalias", "objclass", "objdata", "object", "objname", "objsect", "objtime", "oldcprops",
      "oldpprops", "oldsprops", "oldtprops", "oleclsid", "operator", "panose", "password",
      "passwordhash", "pgp", "pgptbl", "picprop", "pict", "pn", "pnseclvl", "pntext", "pntxta",
      "pntxtb", "printim", "private", "propname", "protend", "protstart", "protusertbl", "pxe",
      "result", "revtbl", "revtim", "rsidtbl", "rxe", "shp", "shpgrp", "shpinst",
      "shppict", "shprslt", "shptxt", "sn", "sp", "staticval", "stylesheet", "subject", "sv",
      "svb", "tc", "template", "themedata", "title", "txe", "ud", "upr", "userprops",
      "wgrffmtfilter", "windowcaption", "writereservation", "writereservhash", "xe", "xform",
      "xmlattrname", "xmlattrvalue", "xmlclose", "xmlname", "xmlnstbl",
      "xmlopen"
   };

   private static StringHash specialCharacters = new(true)
   {
      { "par", "\n" },
      { "sect", "\n\n" },
      { "page", "\n\n" },
      { "line", "\n" },
      { "tab", "\t" },
      { "emdash", "\u2014" },
      { "endash", "\u2013" },
      { "emspace", "\u2003" },
      { "enspace", "\u2002" },
      { "qmspace", "\u2005" },
      { "bullet", "\u2022" },
      { "lquote", "\u2018" },
      { "rquote", "\u2019" },
      { "ldblquote", "\u201C" },
      { "rdblquote", "\u201D" },
   };

   public static Optional<string> stripRichTextFormat(string inputRtf)
   {
      if (inputRtf.IsEmpty())
      {
         return fail("Input string cannot be null");
      }

      var stack = new MaybeStack<StackEntry>();
      var ignorable = false;
      var numberOfAsciiToSkipAfterUnicode = 1;
      var numberOfAsciiLeftToSkip = 0;
      var outputBuffer = new List<string>();

      var _matches = inputRtf.Matches(pattern);
      if (_matches is (true, var matches))
      {
         foreach (var match in matches)
         {
            var word = match.FirstGroup;
            var arg = match.SecondGroup;
            var hex = match.ThirdGroup;
            var character = match.FourthGroup;
            var brace = match.FifthGroup;
            var tChar = match.SixthGroup;

            if (brace.IsNotEmpty())
            {
               numberOfAsciiLeftToSkip = 0;
               switch (brace)
               {
                  case "{":
                     stack.Push(new StackEntry(numberOfAsciiToSkipAfterUnicode, ignorable));
                     break;
                  case "}":
                  {
                     if (stack.Pop() is (true, var entry))
                     {
                        numberOfAsciiToSkipAfterUnicode = entry.NumberOfCharactersToSkip;
                        ignorable = entry.Ignorable;
                     }

                     break;
                  }
               }
            }
            else if (character.IsNotEmpty())
            {
               numberOfAsciiLeftToSkip = 0;
               if (character == "~")
               {
                  if (!ignorable)
                  {
                     outputBuffer.Add("\xA0");
                  }
               }
               else if ("{}\\".Contains(character))
               {
                  if (!ignorable)
                  {
                     outputBuffer.Add(character);
                  }
               }
               else if (character == "*")
               {
                  ignorable = true;
               }
            }
            else if (word.IsNotEmpty())
            {
               numberOfAsciiLeftToSkip = 0;
               var _word = lazy.maybe<string>();
               if (destinations.Contains(word))
               {
                  ignorable = true;
               }
               else if (ignorable)
               {
               }
               else if (_word.ValueOf(specialCharacters.Maybe[word]) is (true, var wordValue))
               {
                  outputBuffer.Add(wordValue);
               }
               else if (word == "uc")
               {
                  numberOfAsciiToSkipAfterUnicode = Value.Int32(arg);
               }
               else if (word == "u")
               {
                  var c = Value.Int32(arg);
                  if (c < 0)
                  {
                     c += 0x10000;
                  }

                  outputBuffer.Add(char.ConvertFromUtf32(c));
                  numberOfAsciiLeftToSkip = numberOfAsciiToSkipAfterUnicode;
               }
            }
            else if (hex.IsNotEmpty())
            {
               if (numberOfAsciiLeftToSkip > 0)
               {
                  numberOfAsciiLeftToSkip -= 1;
               }
               else if (!ignorable)
               {
                  var c = Value.Int32(hex, 0, NumberStyles.HexNumber);
                  outputBuffer.Add(char.ConvertFromUtf32(c));
               }
            }
            else if (tChar.IsNotEmpty())
            {
               if (numberOfAsciiLeftToSkip > 0)
               {
                  numberOfAsciiLeftToSkip = -1;
               }
               else if (!ignorable)
               {
                  outputBuffer.Add(tChar);
               }
            }
         }
      }
      else
      {
         return inputRtf;
      }

      return outputBuffer.ToString("");
   }
}