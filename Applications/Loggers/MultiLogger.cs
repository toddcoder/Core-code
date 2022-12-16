using Core.Collections;

namespace Core.Applications.Loggers;

public class MultiLogger
{
   protected AutoStringHash<Logger> loggers;

   public MultiLogger(int indentation = 2)
   {
      loggers = new AutoStringHash<Logger>(true, _ => new Logger(indentation));
   }
}