using System;

namespace TestTask
{
  public class TokenEmbed : Token
  {
    public TokenEmbed()
    {
      this.start = @"<embed\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"";

      this._skippable = true;
      this._type = "object";
    }

    public override Object Clone()
    {
      return new TokenEmbed();
    }
  }
}