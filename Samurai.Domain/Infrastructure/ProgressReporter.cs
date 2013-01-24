using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;

namespace Samurai.Domain.Infrastructure
{
  public interface IReport
  {
    event EventHandler<ReportTaskAsyncExProgress> Report;
  }

  public interface IProgressReporter
  {
    void ReportProgress(string text, ReporterImportance importance);
  }

  public class ProgressReporter : IProgressReporter
  {
    private readonly IProgress<ReportTaskAsyncExProgress> progress;

    public ProgressReporter(IProgress<ReportTaskAsyncExProgress> progress)
    {
      if (progress == null) throw new ArgumentNullException("progress");
      this.progress = progress;
    }

    public void ReportProgress(string text, ReporterImportance importance)
    {
      var args = new ReportTaskAsyncExProgress()
      {
        Text = text,
        Importance = importance
      };
      this.progress.Report(args);
    }
  }

  public class ReportTaskAsyncExProgress
  {
    public string Text { get; set; }
    public ReporterImportance Importance { get; set; }
  }

}
