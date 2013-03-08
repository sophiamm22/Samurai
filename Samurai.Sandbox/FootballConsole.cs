using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Castle.Windsor;

using Samurai.Domain.Exceptions;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Domain.Entities;
using Samurai.Domain.Infrastructure;
using Samurai.Domain.Model;

namespace Samurai.Sandbox
{
  public class FootballConsole
  {
    private readonly IFootballFacadeAdminService footballService;

    public IEnumerable<FootballFixtureViewModel> Fixtures { get; set; }

    public FootballConsole(IFootballFacadeAdminService footballService)
    {
      if (footballService == null) throw new ArgumentNullException("footballService");

      this.footballService = footballService;
    }

    public void FootballMenu()
    {
      while (true)
      {
        ProgressReporterProvider.Current.ReportProgress("Value-Samurai -- Football Menu", ReporterImportance.High, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("1.\tFetch Day's Schedule", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("2.\tFetch Day's Results", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("3.\tReturn to main menu", ReporterImportance.Low, ReporterAudience.Admin);

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
            FetchFootballSchedule();
            break;
          }
          else if (number == 2)
          {
            FetchFootballResults();
          }
          else
            break;
        }
      }
    }

    private void FetchFootballResults()
    {
      ProgressReporterProvider.Current.ReportProgress("Enter the date to fetch football results (dd/mm/yy)", ReporterImportance.High, ReporterAudience.Admin);

      var dateString = Console.ReadLine();
      DateTime date;
      if (!DateTime.TryParse(dateString, out date))
      {
        Console.WriteLine("You fucking moron!");
        return;
      }
      Fixtures = this.footballService.UpdateDaysResults(date);
    }
    private void FetchFootballSchedule()
    {
      while (true)
      {
        ProgressReporterProvider.Current.ReportProgress("Enter the date to fetch football schedule (dd/mm/yy)", ReporterImportance.High, ReporterAudience.Admin);
        var dateString = Console.ReadLine();
        DateTime date;
        if (!DateTime.TryParse(dateString, out date))
        {
          Console.WriteLine("You fucking moron!");
          break;
        }
        
        var missingURLs = new List<MissingTournamentCouponURL>();
        var missingTeamPlayerAlias = new List<MissingTeamPlayerAlias>();
        var missingBookmakerAlias = new List<MissingBookmakerAlias>();
        try
        {
          Fixtures = this.footballService.UpdateDaysSchedule(date);
          break;
        }
        catch (TournamentCouponURLMissingException tcmEx)
        {
          missingURLs.AddRange(tcmEx.MissingData);
        }
        catch (MissingTeamPlayerAliasException mtpaEx)
        {
          missingTeamPlayerAlias.AddRange(mtpaEx.MissingAlias);
        }
        catch (MissingBookmakerAliasException mbaEx)
        {
          missingBookmakerAlias.AddRange(mbaEx.MissingAlias);
          throw new NotImplementedException();
        }

        AddMissingAlias(missingTeamPlayerAlias, date);
      }
    }


    private void AddMissingAlias(IEnumerable<MissingTeamPlayerAlias> missingAlias, DateTime date)
    {
      var groupedAlias =
        (from alias in missingAlias
         group alias by alias.Tournament into tournamentGroups
         select new
         {
           Tournament = tournamentGroups.Key,
           ExternalSourceGroups =
             from tournamentGroup in tournamentGroups
             group tournamentGroup by tournamentGroup.ExternalSource into sourceGroup
             select new
             {
               ExternalSource = sourceGroup.Key,
               TeamsOrPlayers = sourceGroup.Select(x => x.TeamOrPlayerName)
             }
         })
        .ToList();
      groupedAlias.ForEach(x =>
      {
        var tournamentLadder = this.footballService.GetTournamentLadder(date, x.Tournament);
        foreach (var externalSourceGroup in x.ExternalSourceGroups)
        {
          foreach (var teamOrPlayer in externalSourceGroup.TeamsOrPlayers)
          {
            var team = GetMissingAlias(tournamentLadder, externalSourceGroup.ExternalSource, teamOrPlayer);

            this.footballService.AddAlias(externalSourceGroup.ExternalSource, teamOrPlayer, team);
          }
        }
      });
    }
    private string GetMissingAlias(IEnumerable<FootballLadderViewModel> tournamentLadder, string source, string playerName)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Select a team from the list of {0}", tournamentLadder.Count()), ReporterImportance.High, ReporterAudience.Admin);

      Console.WriteLine();
      var count = 1;
      tournamentLadder.ToList()
                      .ForEach(x => 
                      {
                        ProgressReporterProvider.Current.ReportProgress(string.Format("{0}\t{1}", count, x.TeamName), ReporterImportance.Medium, ReporterAudience.Admin);

                        count++;
                      });
      ProgressReporterProvider.Current.ReportProgress(string.Format("..or enter the team's local name for {0} via {1}", playerName, source), ReporterImportance.Medium, ReporterAudience.Admin);
      Console.WriteLine();
      var response = Console.ReadLine();
      if (Regex.IsMatch(response, @"\d+"))
      {
        var player = tournamentLadder.ElementAt(int.Parse(response) - 1);
        return player.TeamName;
      }
      else
        return response;
    }
  }
}
