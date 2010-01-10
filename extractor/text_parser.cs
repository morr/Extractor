using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestTask
{

  public class TextParser
  {
    private List<Token> _tokens;
    private List<Token> _parsed_data;
    private Stack<string> _parse_stack;
    private string _tokenStart;

    public TextParser(List<Token> tokens)
    {
      this._tokens = tokens;
      this._parsed_data = new List<Token>();
      this._parse_stack = new Stack<String>();

      this._tokenStart = "";
      foreach (Token token in this._tokens)
        this._tokenStart += this._tokenStart.Length == 0 ? token.start : "|" + token.start;
    }

    public List<Token> Process(String data)
    {
      this.Parse(data, 0);
      return this._parsed_data;
    }

    private int Parse(string text, int position)
    {
      // find token
      Regex rgx = new Regex(this._tokenStart+(this._parse_stack.Count != 0 ? "|"+this._parse_stack.Peek() : ""), RegexOptions.IgnoreCase);
      string test = this._tokenStart + (this._parse_stack.Count != 0 ? "|" + this._parse_stack.Peek() : "");
      Match match = rgx.Match(text, position);
      // return if nothing is found
      if (!match.Success)
        return -1;

      int start = match.Index;
      // return if token's end is found
      if (this._parse_stack.Count != 0 && String.Compare(match.Value, this._parse_stack.Peek(), true) == 0)
      {
        this._parse_stack.Pop();
        return start;
      }

      // build new token
      Token token = this.BuildToken(match.Value);
      token.ProcessAttributes(match.Value);

      // find token's end
      int end;
      if (token.end != "")
      {
        this._parse_stack.Push(token.end);

        // process tokens inside this token
        if (!token.skippable)
          end = this.Parse(text, start + match.Value.Length);
        // or just find token's end
        else
          end = token.endRegex.Match(text, start).Index;
        if (end == -1)
          return -1;

        token.ProcessInnerHTML(text.Substring(start + match.Value.Length, end - start - match.Value.Length));
      }
      else
        end = start + match.Value.Length - 1;

      // add token    
      if( token.url != "" )
        this._parsed_data.Add(token);

      // find next tokens
      int stack_size;
      do
      {
        stack_size = this._parse_stack.Count;
        end = this.Parse(text, end + 1);
      }
      while (stack_size == this._parse_stack.Count && end != -1);
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