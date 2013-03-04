using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Windsor;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;

namespace Samurai.Sandbox
{
  public class SamuraiConsole
  {
    private readonly IFootballFacadeService footballService;
    private readonly ITennisFacadeService tennisService;

    public SamuraiConsole(IFootballFacadeService footballService, ITennisFacadeService tennisService)
    {
      if (footballService == null) throw new ArgumentNullException("footballService");
      if (tennisService == null) throw new ArgumentNullException("tennisService");

      this.footballService = footballService;
      this.tennisService = tennisService;

    }

    public void SamuraiMenu()
    {
      while (true)
      {
        ProgressReporterProvider.Current.ReportProgress("Value-Samurai -- Main Menu", ReporterImportance.High);
        ProgressReporterProvider.Current.ReportProgress("1.\tTennis console", ReporterImportance.Medium);
        ProgressReporterProvider.Current.ReportProgress("2.\tFootball console", ReporterImportance.Medium);
        ProgressReporterProvider.Current.ReportProgress("", ReporterImportance.Medium);
        ProgressReporterProvider.Current.ReportProgress("3.\tExit", ReporterImportance.Low);

        var numberString = Console.ReadLine();

        int number;
        if (!int.TryParse(numberString, out number))
        {
          Console.WriteLine("You fucking moron!");
        }
        else
        {
          if (number == 1)
          {
            var tennisConsole = new TennisConsole(tennisService);
            tennisConsole.TennisMenu();
          }
          else if (number == 2)
          {
            var footballConsole = new FootballConsole(footballService);
            footballConsole.FootballMenu();
          }
          else
            break;
        }
      }
    }

  }
}
