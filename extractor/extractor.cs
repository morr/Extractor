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
        string content = (new StreamReader(request.GetResponse().GetResponseStream())).ReadToEnd();

        Match meta_content = Regex.Match(content, @"<meta[^>]* 
                                                      (?:http-equiv=(?<dl>""|'|)content-type\k<dl>[^>]*)?
                                                      content=(?<dl>""|'|)[^""'>]*charset=(?<charset>[^""' ]+)[^""'>]*\k<dl>[^>]*
                                                      (?:http-equiv=(?<dl>""|'|)content-type\k<dl>[^>]*)?
                                                    [^>]*>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        if (meta_content.Success && String.Compare(meta_content.Groups["charset"].Value, "utf-8", true) != 0)
        {
          try
          {
            //Encoding charset = Encoding.GetEncoding(1251);
            //content = Encoding.UTF8.GetString(charset.GetBytes(content));
            //content = Encoding.UTF8.GetString(Encoding.Convert(charset, Encoding.UTF8, charset.GetBytes(content)));

            //Encoding cur_enc = (new StreamReader(request.GetResponse().GetResponseStream())).CurrentEncoding;
            Encoding charset = Encoding.GetEncoding(meta_content.Groups["charset"].Value);
            //content = (new StreamReader(request.GetResponse().GetResponseStream())).ReadToEnd();
            //content = Encoding.UTF8.GetString(charset.GetBytes(content));
            content = Encoding.UTF8.GetString(Encoding.Convert(charset, Encoding.UTF8, Encoding.UTF8.GetBytes(content)));
          }
          catch { }
        }

        TextParser parser = new TextParser(tokens);
        List<Token> data = parser.Process(content);

        Regex domain_test = new Regex(@"https?://(?:[^:]*:[^@]*@)?(:?www.)?([^/]+).*");
        string source_domain = domain_test.Replace(source, "$2");

        StreamWriter sw = new StreamWriter(this._dst, true, Encoding.UTF8);
        Console.WriteLine(source);
        sw.WriteLine(source);
        foreach (Token item in data)
        {
          item.zone = domain_test.Replace(item.url, "$2") == source_domain ? "local" : "remote";
          Console.WriteLine("    {0}", item.ToString());
          sw.WriteLine("    {0}", item.ToString());
        }
        sw.Close();
      }
    }
  }
}