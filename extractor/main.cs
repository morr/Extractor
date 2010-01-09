using System;
using System.IO;

namespace TestTask
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("Incorrect params num.");
        return;
      }
      if (!File.Exists(args[0]))
      {
        Console.WriteLine("In file doesn't exist.");
        return;
      }

      Extractor extractor = new Extractor(args[0], args[1]);
      extractor.Run();

      Console.WriteLine("");
      Console.WriteLine("done");
      Console.ReadKey();
    }
  }
}
