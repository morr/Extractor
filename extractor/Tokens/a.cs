using System;

namespace TestTask
{
  public class TokenA : Token
  {
    public TokenA()
    {
      this.start = @"<a\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"</a>";

      this._type = "page";
    }

    public override void ProcessAttributes(string value)
    {
      base.ProcessAttributes(value);
      if (this._url == "#")
        this._url = "";
    }

    public override Object Clone()
    {
      return new TokenA();
    }
  }
}