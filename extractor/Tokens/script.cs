using System;

namespace TestTask
{
  public class TokenScript : Token
  {
    public TokenScript()
    {
      this.start = @"<script\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"</script>";

      this._skippable = true;
      this._type = "script";
    }

    public override Object Clone()
    {
      return new TokenScript();
    }
  }
}
