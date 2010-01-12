using System;
using System.Text.RegularExpressions;

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

    public override void ProcessAttributes(string value, string base_)
    {
      base.ProcessAttributes(value, base_);
      if (this._url == base_ + '#')
        this._url = "";
      // mailto test
      if (this._url.IndexOf("mailto:") != -1)
      {
        Match match = Regex.Match(value, @"(?:href=|src=)(?:""(?<url>[^""]*)""|'(?<url>[^']*)'|(?<url>[^""'> ]*))", RegexOptions.IgnoreCase);
        if (match.Success)
        {
          string url = System.Web.HttpUtility.UrlPathEncode(match.Groups["url"].Value);
          if (url.IndexOf("mailto:") == 0)
          {
            this._url = url.Remove(0, 8);
            this._type = "mail";
            this._zone = "remote";
          }
        }
      }
    }

    public override Object Clone()
    {
      return new TokenA();
    }
  }
}