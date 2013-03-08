using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Samurai.Domain.Model;

namespace Samurai.Domain.Infrastructure
{
  //an ambient context as seen in Dependency Injection in .Net - 
  //probably not the correct thing to do but the alternative was to wire up all my services with a reporter
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
    public abstract void ReportProgress(string message, ReporterImportance importance, ReporterAudience audience);
  }

  public class DefaultProgressReporterProvider : ProgressReporterProvider
  {
    public override void ReportProgress(string message, ReporterImportance importance, ReporterAudience audience)
    {
      Debug.Print(string.Format("Audience: {0}\tImportance: {1}\tMessage: {2}",
        Enum.GetName(typeof(ReporterAudience), audience), Enum.GetName(typeof(ReporterImportance), importance), message));
    }
  }
}
