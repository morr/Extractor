using System;

namespace TestTask
{
  public class TokenComment : Token
  {
    public TokenComment()
    {
      this.start = @"<!--";
      this.end = @"-->";
      this._skippable = true;
    }

    public override Object Clone()
    {
      return new TokenComment();
    }
  }
}
