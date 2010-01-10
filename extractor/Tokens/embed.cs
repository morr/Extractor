using System;

namespace TestTask
{
  public class TokenEmbed : Token
  {
    public TokenEmbed()
    {
      this.start = @"<enbed\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"</embed>";

      this._skippable = true;
      this._type = "object";
    }

    public override Object Clone()
    {
      return new TokenEmbed();
    }
  }
}