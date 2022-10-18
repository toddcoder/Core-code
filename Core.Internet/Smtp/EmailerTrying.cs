using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Internet.Smtp;

public class EmailerTrying
{
   protected Emailer emailer;

   public EmailerTrying(Emailer emailer) => this.emailer = emailer;

   public Result<Unit> SendText() => tryTo(() => emailer.SendText());

   public Result<Unit> SendHtml() => tryTo(() => emailer.SendHtml());

   public Result<Unit> SendAsHtmlIf(bool isHTML) => tryTo(() => emailer.SendAsHtmlIf(isHTML));

   public Result<Unit> SendAsTextIf(bool isText) => tryTo(() => emailer.SendAsTextIf(isText));
}