using System;
using Core.Computers;
using Core.Dates.DateIncrements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class CommandTextBuilder
{
   public static CommandTextBuilder operator +(CommandTextBuilder build, SqlSetupBuilderParameters.ICommandTextParameter parameter)
   {
      return parameter switch
      {
         SqlSetupBuilderParameters.CommandText commandText => build.CommandText(commandText),
         SqlSetupBuilderParameters.CommandTextFile commandTextFile => build.CommandTextFile(commandTextFile),
         SqlSetupBuilderParameters.CommandTimeout commandTimeout => build.CommandTimeout(commandTimeout),
         _ => throw new ArgumentOutOfRangeException(nameof(parameter))
      };
   }

   protected SqlSetupBuilder setupBuilder;
   protected Maybe<string> _commandText;
   protected Maybe<FileName> _commandTextFile;
   protected Maybe<TimeSpan> _commandTimeout;

   public CommandTextBuilder(SqlSetupBuilder setupBuilder)
   {
      this.setupBuilder = setupBuilder;
      this.setupBuilder.CommandTextBuilder(this);

      _commandText = nil;
      _commandTextFile = nil;
      _commandTimeout = nil;
   }

   public CommandTextBuilder CommandText(string commandText)
   {
      _commandText = commandText;
      return this;
   }

   public CommandTextBuilder CommandTextFile(FileName commandTextFile)
   {
      _commandTextFile = commandTextFile;
      return this;
   }

   public CommandTextBuilder CommandTimeout(TimeSpan commandTimeout)
   {
      _commandTimeout = commandTimeout;
      return this;
   }

   public Result<(string, TimeSpan)> Build()
   {
      var commandTimeout = _commandTimeout | (() => 30.Seconds());
      if (_commandText)
      {
         return (_commandText, commandTimeout);
      }
      else if (_commandTextFile)
      {
         return _commandTextFile.Value.TryTo.Text.Map(ct => (ct, commandTimeout));
      }
      else
      {
         return fail("No command text provided");
      }
   }
}