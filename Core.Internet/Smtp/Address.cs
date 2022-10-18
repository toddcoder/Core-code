using System.Collections.Generic;
using Core.Configurations;
using Core.Enumerables;
using Core.Matching;
using Core.Strings;

namespace Core.Internet.Smtp;

public class Address
{
   protected static string joinByCommas(string value) => value.Unjoin("/s* [';,'] /s*; f").ToString(", ");

   protected string to;
   protected string cc;
   protected string bcc;

   public Address()
   {
      Server = string.Empty;
      From = string.Empty;
      to = string.Empty;
      cc = string.Empty;
      bcc = string.Empty;
      Subject = string.Empty;
   }

   public Address(Setting addressSetting)
   {
      Server = addressSetting.Value.String("server");
      From = addressSetting.Value.String("from");
      To = addressSetting.Value.String("to");
      CC = addressSetting.Value.String("cc");
      BCC = addressSetting.Value.String("bcc");
      Subject = addressSetting.Value.String("subject");
   }

   public string Server { get; set; }

   public string From { get; set; }

   public string To
   {
      get => to;
      set => to = joinByCommas(value);
   }

   public string CC
   {
      get => cc;
      set => cc = joinByCommas(value);
   }

   public string BCC
   {
      get => bcc;
      set => bcc = joinByCommas(value);
   }

   public string Subject { get; set; }

   public Address Clone(string subject) => new()
   {
      Server = Server.Copy(),
      From = From.Copy(),
      To = to.Copy(),
      CC = cc.Copy(),
      BCC = bcc.Copy(),
      Subject = subject
   };

   public Address Clone() => Clone(Subject.Copy());

   public override string ToString()
   {
      var result = new List<string>();

      if (Server.IsNotEmpty())
      {
         result.Add(Server);
      }

      if (From.IsNotEmpty())
      {
         result.Add(From);
      }

      if (To.IsNotEmpty())
      {
         result.Add(to);
      }

      if (CC.IsNotEmpty())
      {
         result.Add(cc);
      }

      if (BCC.IsNotEmpty())
      {
         result.Add(bcc);
      }

      if (Subject.IsNotEmpty())
      {
         result.Add(Subject);
      }

      return result.ToString(", ");
   }
}