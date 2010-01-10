using System;

namespace TestTask
{
  public class TokenLink : Token
  {
    public TokenLink()
    {
      this.start = @"<link\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"";

      this._skippable = true;
      this._type = "css";
    }

    public override Object Clone()
    {
      return new TokenLink();
    }
  }
}