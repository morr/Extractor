using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace TestTask
{
  public static class PageLoader
  {
    public static string Load(string url)
    {
      try
      {
        // accept all certificates
        if(url.IndexOf("https://") == 0)
          ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        // request
        WebRequest request = WebRequest.Create(url);
        // http authentication
        Match auth = Regex.Match(url, @"https?://(?<login>[^:]*):(?<password>[^@]*)@(:?www.)?([^/]+).*", RegexOptions.IgnoreCase);
        if (auth.Success)
          request.Credentials = new NetworkCredential(auth.Groups["login"].Value, auth.Groups["password"].Value);
        // response
        WebResponse response = request.GetResponse();

        // encoding
        Encoding encoding = Encoding.GetEncoding(1251);
        Match charset = Regex.Match(response.ContentType, "charset=(?<charset>[^ ]+)", RegexOptions.IgnoreCase);
        if (charset.Success)
          encoding = Encoding.GetEncoding(charset.Groups["charset"].Value);
        // content
        string content = (new StreamReader(response.GetResponseStream(), encoding)).ReadToEnd();

        return content;
      }
      catch
      {
        return "";
      }
    }
  }
}