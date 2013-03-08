using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;

namespace Samurai.Sandbox
{
  public class ConsoleProgressReporterProvider : ProgressReporterProvider
  {
    public override void ReportProgress(string message, ReporterImportance importance, ReporterAudience audience)
    {
      if (Console.WindowWidth != 145) 
        Console.WindowWidth = 145;

      if (importance == ReporterImportance.High)
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine(new String('-', message.Length));
        Console.WriteLine();
      }
      else if (importance == ReporterImportance.Medium)
      {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
      }
      else if (importance == ReporterImportance.Low)
      {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
      }

    }
  }
}
