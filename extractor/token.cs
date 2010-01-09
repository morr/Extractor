using System;
using System.Text.RegularExpressions;

namespace TestTask
{
  public enum TagZone
  {
    local, remote
  }

  public enum TagType
  {
    page, image, css, script
  }

  public abstract class Token : ICloneable
  {
    protected string _start;
    public string start
    {
      get { return this._start; }
      set
      {
        this._start = value;
        this._startRegex = new Regex(value, RegexOptions.IgnoreCase);
      }
    }
    protected string _end;
    public string end
    {
      get { return this._end; }
      set
      {
        this._end = value;
        this._endRegex = new Regex(value, RegexOptions.IgnoreCase);
      }
    }

    protected Regex _startRegex;
    public Regex startRegex
    {
      get { return this._startRegex; }
    }
    protected Regex _endRegex;
    public Regex endRegex
    {
      get { return this._endRegex; }
    }

    protected TagZone _zone;
    protected TagType _type;

    protected string _url;
    public string url
    {
      get { return this._url; }
      set
      {
        Match match = Regex.Match(value, @"(?:href=|src=)(?:""(?<url>[^""]*)""|'(?<url>[^']*)'|(?<url>[^""'> ]*))", RegexOptions.IgnoreCase);
        if (match.Success)
          this._url = match.Groups["url"].Value;
        else
          this._url = value;
      }
    }

    protected string _innerHTML;
    public string innerHTML
    {
      get { return this._innerHTML; }
      set
      {
        this._innerHTML = Regex.Replace(value, @"<(?:""[^""]*""|'[^']*'|[^""'>])*>", "");
      }
    }

    protected bool _skippable;
    public bool skippable
    {
      get { return this._skippable; }
    }

    public Token()
    {
      this._skippable = false;
    }

    abstract public Object Clone();
  }


  public class TokenA : Token
  {
    public TokenA()
    {
      this.start = @"<a\b(?:""[^""]*""|'[^']*'|[^""'>])*>";
      this.end = @"</a>";
    }

    public override Object Clone()
    {
      return new TokenA();
    }
  }

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