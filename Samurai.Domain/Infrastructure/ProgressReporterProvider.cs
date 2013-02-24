using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Samurai.Domain.Model;

namespace Samurai.Domain.Infrastructure
{
  //an ambient context as seen in Dependency Injection in .Net - probably not the correct thing to do!
  public abstract class ProgressReporterProvider
  {
    private static ProgressReporterProvider current;

    static ProgressReporterProvider()
    {
      ProgressReporterProvider.current = new DefaultProgressReporterProvider();
    }

    public static ProgressReporterProvider Current
    {
      get { return ProgressReporterProvider.current; }
      set
      {
        if (value == null) throw new ArgumentNullException("value");
        ProgressReporterProvider.current = value;
      }
    }
    public abstract void ReportProgress(string message, ReporterImportance importance);
  }

  public class DefaultProgressReporterProvider : ProgressReporterProvider
  {
    public override void ReportProgress(string message, ReporterImportance importance)
    {
      Debug.Print(message);
    }
  }
}
