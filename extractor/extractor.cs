using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TestTask
{
  class Extractor
  {
    private string _src;
    private string _dst;

    private List<string> _sources;

    public Extractor(string src, string dst)
    {
      this._src = src;
      this._dst = dst;

      this._sources = new List<string>();

      StreamReader sr = new StreamReader(this._src);
      String line;
      while ((line = sr.ReadLine()) != null)
        _sources.Add(line.Trim());
    }

    public void Run()
    {
      foreach (String source in this._sources)
      {
        // Create a request for the URL. 		
        WebRequest request = WebRequest.Create(source);
        // Get the response.
        string content = (new StreamReader(request.GetResponse().GetResponseStream())).ReadToEnd();

        TextParser parser = new TextParser();
        parser.AddToken(new TokenA());
        parser.AddToken(new TokenComment());
        List<Token> data = parser.Process(content);
      }
    }
  }
}
