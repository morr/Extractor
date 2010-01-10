using System;

namespace TestTask
{
  public class TokenImg : Token
  {
    public TokenImg()
    {
      this.start = @"<img\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"";

      this._type = "image";
    }

    public override Object Clone()
    {
      return new TokenImg();
    }
  }
}