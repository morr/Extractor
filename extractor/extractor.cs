using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;

namespace TestTask
{
  class Extractor
  {
    private string _src;
    private string _dst;

    private Queue<string> _sources;
    private List<Token> _tokens;

    private Mutex _queueLock;
    private Mutex _outputLock;

    public Extractor(string src, string dst)
    {
      this._src = src;
      this._dst = dst;

      this._sources = new Queue<string>();

      StreamReader sr = new StreamReader(this._src);
      String line;
      while ((line = sr.ReadLine()) != null)
        _sources.Enqueue(line.Trim());

      this._queueLock = new Mutex();
      this._outputLock = new Mutex();

      this._tokens = new List<Token>();
      this._tokens.Add(new TokenA());
      this._tokens.Add(new TokenComment());
      this._tokens.Add(new TokenArea());
      this._tokens.Add(new TokenImg());
      this._tokens.Add(new TokenScript());
      this._tokens.Add(new TokenLink());
      this._tokens.Add(new TokenEmbed());
    }

    public void Run(int threads_num)
    {
      List<Thread> threads = new List<Thread>();
      for (int i = 0; i < threads_num; i++)
        threads.Add(new Thread(this.ProcessSources));
      
      foreach (Thread thread in threads)
        thread.Start();
      foreach (Thread thread in threads)
        thread.Join();
    }

    private void ProcessSources()
    {
      while (true)
      {
        // get source
        this._queueLock.WaitOne();
        if (this._sources.Count == 0)
        {
          this._queueLock.ReleaseMutex();
          break;
        }
        string source = this._sources.Dequeue();
        this._queueLock.ReleaseMutex();

        string content = PageLoader.Load(source);

        // parse content
        List<Token> data = (new TextParser(content, source, this._tokens)).parsedTokens;

        // save to file
        this._outputLock.WaitOne();
        StreamWriter sw = new StreamWriter(this._dst, true);
        Console.WriteLine("{0} {1}", System.Web.HttpUtility.UrlPathEncode(source), DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
        sw.WriteLine("{0} {1}", System.Web.HttpUtility.UrlPathEncode(source), DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));
        foreach (Token item in data)
        {
          Console.WriteLine("    {0}", item.ToString());
          sw.WriteLine("    {0}", item.ToString());
        }
        sw.Close();
        this._outputLock.ReleaseMutex();
      }
    }
  }
}