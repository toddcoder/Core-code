using System;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class ParseValueArgs : EventArgs
   {
      public ParseValueArgs(TokenType tokenType, string name, string value)
      {
         TokenType = tokenType;
         Name = name;
         Value = value;
      }

      public TokenType TokenType { get; }

      public string Name { get; }

      public string Value { get; }

      public void Deconstruct(out TokenType tokenType, out string name, out string value)
      {
         tokenType = TokenType;
         name = Name;
         value = Value;
      }
   }
}