using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Internet.Smtp;

public class EmailerTrying
{
   protected Emailer emailer;

   public EmailerTrying(Emailer emailer) => this.emailer = emailer;

   public Optional<Unit> SendText() => tryTo(() => emailer.SendText());

   public Optional<Unit> SendHtml() => tryTo(() => emailer.SendHtml());

   public Optional<Unit> SendAsHtmlIf(bool isHTML) => tryTo(() => emailer.SendAsHtmlIf(isHTML));

   public Optional<Unit> SendAsTextIf(bool isText) => tryTo(() => emailer.SendAsTextIf(isText));
}