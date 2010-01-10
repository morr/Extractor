using System;

namespace TestTask
{
  public class TokenArea : Token
  {
    public TokenArea()
    {
      this.start = @"<area\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"</area>";

      this._type = "page";
    }

    public override Object Clone()
    {
      return new TokenArea();
    }
  }
}