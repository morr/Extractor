using System;
using System.Text.RegularExpressions;

namespace TestTask
{
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

    protected string _zone;
    public string zone
    {
      get { return this._zone; }
      set { this._zone = value; }
    }
    protected string _type;
    public string type
    {
      get { return this._type; }
    }

    protected string _url;
    public string url
    {
      get { return this._url; }
    }

    protected string _innerHTML;
    public string innerHTML
    {
      get { return this._innerHTML; }
    }

    protected bool _skippable;
    public bool skippable
    {
      get { return this._skippable; }
    }

    public Token()
    {
      this.end = "";
      this._innerHTML = "";
      this._skippable = false;
      this.start = "";
      this._type = "";
      this._url = "";
      this._zone = "";
    }

    public virtual void ProcessAttributes(string value)
    {
      Match match = Regex.Match(value, @"(?:href=|src=)(?:""(?<url>[^""]*)""|'(?<url>[^']*)'|(?<url>[^""'> ]*))", RegexOptions.IgnoreCase);
      if (match.Success)
        this._url = match.Groups["url"].Value;
    }

    public virtual void ProcessInnerHTML(string value)
    {
      this._innerHTML = Regex.Replace(value, @"<(?:""[^""]*""|'[^']*'|[^""'>])*>", "");
    }

    public virtual string ToString()
    {
      return this._type+" "+this._zone+" "+this._url+" "+this._innerHTML;
    }

    public abstract Object Clone();
  }
}