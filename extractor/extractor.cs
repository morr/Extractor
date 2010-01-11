using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
      List<Token> tokens = new List<Token>();
      tokens.Add(new TokenA());
      tokens.Add(new TokenArea());
      tokens.Add(new TokenImg());
      tokens.Add(new TokenScript());
      tokens.Add(new TokenLink());
      tokens.Add(new TokenEmbed());

      foreach (String source in this._sources)
      {
        WebRequest request = WebRequest.Create(source);
        WebResponse response = request.GetResponse();
        Encoding encoding = Encoding.GetEncoding(1251);

        Match charset = Regex.Match(response.ContentType, "charset=(?<charset>[^ ]+)", RegexOptions.IgnoreCase);
        if (charset.Success)
        {
          try
          {
            encoding = Encoding.GetEncoding(charset.Groups["charset"].Value);
          }
          catch{}
        }
        string content = (new StreamReader(response.GetResponseStream(), encoding)).ReadToEnd();

        //Match meta_content = Regex.Match(content, @"<meta[^>]* 
        //                                              (?:http-equiv=(?<dl>""|'|)content-type\k<dl>[^>]*)?
        //                                              content=(?<dl>""|'|)[^""'>]*charset=(?<charset>[^""' ]+)[^""'>]*\k<dl>[^>]*
        //                                              (?:http-equiv=(?<dl>""|'|)content-type\k<dl>[^>]*)?
        //                                            [^>]*>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        TextParser parser = new TextParser(tokens);
        List<Token> data = parser.Process(content);

        string source_domain = Regex.Replace(source, @"https?://(?:[^:]*:[^@]*@)?(:?www.)?([^/]+).*", "$2");
        Regex domain_test = new Regex(@"https?://(?:[^:]*:[^@]*@)?(:?www.)?"+source_domain+@"\b");

        StreamWriter sw = new StreamWriter(this._dst, true, Encoding.UTF8);
        Console.WriteLine("{0} {1}", source, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
        sw.WriteLine(source);
        foreach (Token item in data)
        {
          item.zone = item.url.IndexOf("://") == -1 || domain_test.Match(item.url).Success ? "local" : "remote";
          Console.WriteLine("    {0}", item.ToString());
          sw.WriteLine("    {0}", item.ToString());
        }
        sw.Close();
      }
    }
  }
}