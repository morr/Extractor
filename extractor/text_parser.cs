using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestTask
{

  public class TextParser
  {
    private List<Token> _tokens;
    private string _tokenStart;

    private List<Token> _parsedTokens;
    public List<Token> parsedTokens
    {
      get { return this._parsedTokens; }
    }

    private Stack<string> _parseStack;

    private string _domain;
    private string _base;

    private Regex _domainTest;

    public TextParser(string data, string url, List<Token> tokens) 
    {
      this._tokens = tokens;
      this._parsedTokens = new List<Token>();
      this._parseStack = new Stack<String>();

      this._tokenStart = "";
      foreach (Token token in this._tokens)
        this._tokenStart += this._tokenStart.Length == 0 ? token.start : "|" + token.start;

      Match url_match = Regex.Match(url, @"(?<base>https?://(?:[^:]*:[^@]*@)?(:?www.)?(?<domain>[^/]+)).*", RegexOptions.IgnoreCase);
      this._domain = url_match.Groups["domain"].Value;
      this._domainTest = new Regex(@"https?://(?:[^:]*:[^@]*@)?(:?www.)?"+this._domain+@"\b");
      this._base = url_match.Groups["base"].Value;
      // attempt to find <base> tag in data
      Match mbase = Regex.Match(data, @"<base\s+href=(?:""(?<url>[^""]*)""|'(?<url>[^']*)'|(?<url>[^""'> ]*))[^>]*>", RegexOptions.IgnoreCase);
      if (mbase.Success)
        this._base = mbase.Groups["url"].Value;
      if (!Regex.Match(this._base, "/$").Success)
        this._base += '/';

      this.Parse(data, 0);
    }

    private int Parse(string text, int position)
    {
      // find token
      Regex rgx = new Regex(this._tokenStart+(this._parseStack.Count != 0 ? "|"+this._parseStack.Peek() : ""), RegexOptions.IgnoreCase);
      string test = this._tokenStart + (this._parseStack.Count != 0 ? "|" + this._parseStack.Peek() : "");
      Match match = rgx.Match(text, position);
      // return if nothing is found
      if (!match.Success)
        return -1;

      int start = match.Index;
      // return if token's end is found
      if (this._parseStack.Count != 0 && String.Compare(match.Value, this._parseStack.Peek(), true) == 0)
      {
        this._parseStack.Pop();
        return start;
      }

      // build new token
      Token token = this.BuildToken(match.Value);
      token.ProcessAttributes(match.Value, this._base);
      if (token.zone.Length == 0)
        token.zone = token.url.IndexOf("://") == -1 || this._domainTest.Match(token.url).Success ? "local" : "remote";

      // find token's end
      int end;
      if (token.end != "")
      {
        // process other tokens inside this token and find token's end
        if (!token.skippable)
        {
          this._parseStack.Push(token.end);
          end = this.Parse(text, start + match.Value.Length);
        }
        // or just find token's end
        else
        {
          end = token.endRegex.Match(text, start).Index;
          if (end != -1)
            end -= 1;
        }
        if (end == -1)
          return -1;

        if (end - start - match.Value.Length != -1)
          token.ProcessInnerHTML(text.Substring(start + match.Value.Length, end - start - match.Value.Length));
      }
      else
        end = start + match.Value.Length - 1;

      // add token    
      if( token.url != "" )
        this._parsedTokens.Add(token);

      // find next tokens
      int stack_size;
      do
      {
        stack_size = this._parseStack.Count;
        end = this.Parse(text, end + 1);
      }
      while (stack_size == this._parseStack.Count && end != -1);
      return end;
    }

    private Token BuildToken(string text)
    {
      foreach (Token token in this._tokens)
        if (token.startRegex.IsMatch(text))
        {
          Token tag = (Token)token.Clone();
          return tag;
        }

      throw new Exception("BuildToken error: unexpected name");
    }
  }

}